using FrontDesk.Api.Config;
using FrontDesk.Api.Middleware;
using FrontDesk.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.AddControllers();
builder.AddJsonConfig();
builder.AddDbConfig();
var ratePolicy = builder.AddRateLimitingConfig();
builder.Services.AddScoped<IVisitService, VisitService>();
var app = builder.Build();

// Initialization
await app.MigrateDbAsync();

// Pipeline
app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.MapGet("/health", () => Results.Ok("Healthy"))
    .RequireRateLimiting(ratePolicy);
app.MapControllers()
    .RequireRateLimiting(ratePolicy);
app.Run();

// For integration tests.
public partial class Program { }