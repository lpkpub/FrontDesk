using FrontDesk.Db;
using FrontDesk.Db.Tables;
using FrontDesk.Shared;
using FrontDesk.Shared.Dto;
using FrontDesk.Shared.Enums;
using FrontDesk.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace FrontDesk.Api.Services;

public class VisitService : IVisitService
{
    private readonly FrontDeskDbc _dbc;
    private readonly ILogger<VisitService> _logger;

    public VisitService(FrontDeskDbc dbc, ILogger<VisitService> logger)
    {
        _dbc = dbc;
        _logger = logger;
    }

    public async Task<VisitResponse> ProcessVisitAsync(VisitRequest req) =>
        req.UserType switch
        {
            UserType.Staff => await ProcessVisit_StaffAsync(req),
            UserType.Visitor => req.VisitAction switch
            {
                VisitAction.Entry => await ProcessVisit_Visitor_EntryAsync(req),
                VisitAction.Exit => await ProcessVisit_Visitor_ExitAsync(req),
                _ => VisitResponse.Rejected()
            },
            _ => VisitResponse.Rejected()
        };

    /// <summary>
    /// Staff Entry/Exit is determined by the presence of a Visit on the same day. <br/>
    /// Uses <see cref="VisitRequest.Pin"/> as lookup.
    /// </summary>
    private async Task<VisitResponse> ProcessVisit_StaffAsync(VisitRequest req)
    {
        // Confirm valid Pin.
        if (!PinValidator.IsValid(req.Pin))
            return VisitResponse.Rejected();

        // Confirm valid user.
        var user = await _dbc.User
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Pin == req.Pin);
        if (user is null || user.UserType == UserType.Visitor || !user.IsActive)
            return VisitResponse.Rejected();

        var currentVisit = await _dbc.Visit
            .Where(x => x.UserId == user.Id && x.ExitAt == null)
            .OrderByDescending(x => x.EntryAt)
            .FirstOrDefaultAsync();

        VisitAction visitAction = default;
        if (currentVisit is null)
        {
            visitAction = VisitAction.Entry;
            await CreateStaffEntryAsync(user.Id);
        }
        else
        {
            // Exit if EntryAt is on same day, otherwise new entry.
            var currentDate = LocalDate(DateTime.UtcNow);
            var currentVisitDate = LocalDate(currentVisit.EntryAt);
            if (currentVisitDate == currentDate)
            {
                visitAction = VisitAction.Exit;
                await UpdateStaffExitAsync(currentVisit);
            }
            else
            {
                visitAction = VisitAction.Entry;
                await CreateStaffEntryAsync(user.Id);
            }
        }

        return VisitResponse.Accepted(visitAction, user.Name);
    }

    private async Task CreateStaffEntryAsync(Guid userId)
    {
        var visit = new Visit()
        {
            EntryAt = DateTime.UtcNow,
            ExitAt = null,
            UserId = userId,
            Company = Const.INTERNAL,
            PhoneNumber = null,
            Reason = Const.SHIFT
        };

        _dbc.Add(visit);
        await _dbc.SaveChangesAsync();
    }

    private async Task UpdateStaffExitAsync(Visit visit)
    {
        visit.ExitAt = DateTime.UtcNow;
        _dbc.Update(visit);
        await _dbc.SaveChangesAsync();
    }

    /// <summary>
    /// Creates new user, including a unique temporary pin for one-off exit.
    /// </summary>
    private async Task<VisitResponse> ProcessVisit_Visitor_EntryAsync(VisitRequest req)
    {
        string pin;
        try
        {
            async Task<bool> isPinTakenAsync(string pin) => await _dbc.User.AnyAsync(u => u.Pin == pin);
            pin = await PinGenerator.GenerateUniqueAsync(isPinTakenAsync);
        }
        catch (InvalidOperationException ex) // Thrown by GenerateUniqueAsync after max attempts.
        {
            _logger.LogError(ex, "Failed to generate unique PIN.");
            return VisitResponse.Rejected();
        }

        var user = new User
        {
            Name = req.Name,
            Pin = pin,
            UserType = UserType.Visitor,
            IsActive = true
        };
        _dbc.Add(user);

        var visit = new Visit
        {
            EntryAt = DateTime.UtcNow,
            ExitAt = null,
            User = user,
            Company = req.Company,
            PhoneNumber = req.PhoneNumber,
            Reason = req.Reason
        };
        _dbc.Add(visit);

        try
        {
            await _dbc.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "PIN collision on save.");
            return VisitResponse.Rejected();
        }

        return VisitResponse.Accepted(VisitAction.Entry, user.Name, pin);
    }

    /// <summary>
    /// Uses <see cref="VisitRequest.Pin"/> as lookup, and nulls Pin on update.
    /// </summary>
    private async Task<VisitResponse> ProcessVisit_Visitor_ExitAsync(VisitRequest req)
    {
        // Confirm valid Pin.
        if (!PinValidator.IsValid(req.Pin))
            return VisitResponse.Rejected();

        // Confirm valid user.
        var user = await _dbc.User
            .FirstOrDefaultAsync(x => x.Pin == req.Pin);
        if (user is null || user.UserType == UserType.Staff || !user.IsActive)
            return VisitResponse.Rejected();

        // Confirm incomplete visit.
        var currentVisit = await _dbc.Visit
            .Where(x => x.UserId == user.Id && x.ExitAt == null)
            .FirstOrDefaultAsync();
        if (currentVisit is null)
            return VisitResponse.Rejected();

        user.Pin = null;
        user.IsActive = false;
        _dbc.Update(user);

        currentVisit.ExitAt = DateTime.UtcNow;
        _dbc.Update(currentVisit);

        await _dbc.SaveChangesAsync();

        return VisitResponse.Accepted(VisitAction.Exit, user.Name);
    }

    private static DateOnly LocalDate(DateTime dt)
    {
        var visitTimeZone = GetLocalTimeZone();
        var local = TimeZoneInfo.ConvertTime(dt, visitTimeZone);
        return DateOnly.FromDateTime(local);
    }

    private static TimeZoneInfo GetLocalTimeZone()
    {
        // Linux/macOS (IANA)
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(Const.NEW_ZEALAND_TIME_ZONE_IANA);
        }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        // Windows
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(Const.NEW_ZEALAND_TIME_ZONE_WINDOWS);
        }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        throw new InvalidOperationException("Timezone not found.");
    }
}
