using Testcontainers.Oracle;

namespace Demoulas.ProfitSharing.IntegrationTests.Fixtures;

public class OracleContainerFixture : IAsyncLifetime
{
    public OracleContainer OracleContainer { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        OracleContainer = new OracleBuilder()
            .WithImage("gvenzl/oracle-xe:21.3.0-slim-faststart")
            .Build();

        await OracleContainer.StartAsync();

        
    }

    public async Task DisposeAsync()
    {
        await OracleContainer.StopAsync()!;
    }
}