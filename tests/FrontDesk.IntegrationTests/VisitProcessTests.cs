using FrontDesk.Shared;
using FrontDesk.Shared.Dto;
using FrontDesk.Shared.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace FrontDesk.IntegrationTests;

public class VisitProcessTests : IClassFixture<PostgresFixture>
{
    private readonly HttpClient _client;

    public VisitProcessTests(PostgresFixture postgres)
    {
        // Set environment variables to override appsettings/UserSecrets.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", postgres.ConnectionString);
        Environment.SetEnvironmentVariable(Const.API_KEY, "test-key");

        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.TryAddWithoutValidation(Const.API_KEY_HEADER_NAME, "test-key");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ProcessVisit_VisitorEntry_ReturnsAcceptedWithPin()
    {
        var req = new VisitRequest
        {
            UserType = UserType.Visitor,
            VisitAction = VisitAction.Entry,
            Name = "Test Visitor",
            Company = "TestCo",
            PhoneNumber = "0210000000",
            Reason = "Interview"
        };

        var resp = await _client.PostAsJsonAsync("/api/visit/process", req);
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadFromJsonAsync<VisitResponse>();
        Assert.NotNull(body);
        Assert.True(body!.IsAccepted);
        Assert.Equal(VisitAction.Entry, body.VisitAction);
        Assert.False(string.IsNullOrWhiteSpace(body.VisitorPin));
        Assert.Equal(6, body.VisitorPin!.Length);
    }
}