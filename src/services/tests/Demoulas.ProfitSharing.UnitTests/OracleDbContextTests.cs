using Demoulas.Common.Data.Contexts.DTOs.Request;
using Demoulas.ProfitSharing.IntegrationTests.Fixtures;
using Demoulas.ProfitSharing.Services;
using Microsoft.Extensions.DependencyInjection;
using MockDataContextFactory = Demoulas.ProfitSharing.IntegrationTests.Mocks.MockDataContextFactory;

namespace Demoulas.ProfitSharing.IntegrationTests;

public class OracleDbContextTests : IClassFixture<OracleContainerFixture>
{
    private readonly OracleContainerFixture _fixture;
    public OracleDbContextTests(OracleContainerFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task TestInsertAndRetrieveEntity()
    {
        var connectionString = _fixture.OracleContainer.GetConnectionString();
        var factory = MockDataContextFactory.InitializeForTesting(new ServiceCollection(), connectionString);
        var ds = new DemographicsService(factory);

        _ = await ds.GetAllDemographics(new PaginationRequestDto(), CancellationToken.None);

        await Task.CompletedTask;
    }
}
