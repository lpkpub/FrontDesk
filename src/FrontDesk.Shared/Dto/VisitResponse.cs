using FrontDesk.Shared.Enums;

namespace FrontDesk.Shared.Dto;

public sealed class VisitResponse
{
    public bool IsAccepted { get; init; }
    public VisitAction VisitAction { get; init; }
    public string Username { get; init; } = string.Empty;
    public string? VisitorPin { get; init; }

    public static VisitResponse Rejected() => new()
    {
        IsAccepted = false
    };

    public static VisitResponse Accepted(
        VisitAction action,
        string username,
        string? visitorPin = null)
        => new()
        {
            IsAccepted = true,
            VisitAction = action,
            Username = username,
            VisitorPin = visitorPin
        };
}