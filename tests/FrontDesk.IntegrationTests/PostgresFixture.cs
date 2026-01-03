using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;
using Xunit;

namespace FrontDesk.IntegrationTests;

public sealed class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } =
        new PostgreSqlBuilder().Build();

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            await Container.StartAsync();
        }
        catch (DockerUnavailableException)
        {
            throw new Exception("Docker is not running.");
        }
    }

    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}