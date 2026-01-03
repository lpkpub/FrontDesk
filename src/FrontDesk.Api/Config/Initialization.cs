using FrontDesk.Db;
using Microsoft.EntityFrameworkCore;

namespace FrontDesk.Api.Config;

public static class Initialization
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbc = scope.ServiceProvider.GetRequiredService<FrontDeskDbc>();
        var pending = await dbc.Database.GetPendingMigrationsAsync();
        if (!pending.Any())
            return;

        try
        {
            await dbc.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration failed: {ex.Message}");
            throw;
        }

        foreach (var migration in pending)
        {
            Console.WriteLine($"Migration applied: {migration}");
        }
    }
}