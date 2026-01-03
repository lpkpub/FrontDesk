using FrontDesk.Shared;

namespace FrontDesk.Api.Middleware;

/// <summary>
/// Allows access to the /health endpoint without Api key.
/// </summary>
public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _expectedKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _expectedKey = config[Const.API_KEY];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_expectedKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync($"{Const.API_KEY} not found.");
            return;
        }

        if (!context.Request.Headers.TryGetValue(Const.API_KEY_HEADER_NAME, out var providedKey) ||
            providedKey.Count != 1 ||
            providedKey[0] != _expectedKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync($"Missing or invalid {Const.API_KEY}.");
            return;
        }

        await _next(context);
    }
}