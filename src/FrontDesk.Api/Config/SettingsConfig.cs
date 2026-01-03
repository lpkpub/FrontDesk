using FrontDesk.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FrontDesk.Api.Config;

public static class SettingsConfig
{
    public static void AddJsonConfig(this WebApplicationBuilder builder)
    {
        // To keep Pascal casing of C# classes.
        builder.Services.Configure<JsonOptions>(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
    }

    public static void AddDbConfig(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        builder.Services.AddDbContext<FrontDeskDbc>(options =>
        options
        .UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention()
        .ConfigureWarnings(w => w.Log(RelationalEventId.PendingModelChangesWarning)));
    }

    public static string AddRateLimitingConfig(this WebApplicationBuilder builder)
    {
        var policyName = "global";

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(policyName, limiterOptions =>
            {
                limiterOptions.PermitLimit = 20;
                limiterOptions.Window = TimeSpan.FromSeconds(10);
                limiterOptions.QueueLimit = 0;
            });
        });

        return policyName;
    }
}