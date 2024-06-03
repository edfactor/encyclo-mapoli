using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Base;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.IntegrationTests;

public class DemographicsServiceTests : IClassFixture<ApiTestBase<Program>>
{
    private readonly ApiTestBase<Program> _fixture;

    public DemographicsServiceTests(ApiTestBase<Program> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestInsertAndRetrieveEntity()
    {
        var ds = _fixture.ServiceProvider!.GetRequiredService<IDemographicsService>();

        var response = await ds.GetAllDemographics(new PaginationRequestDto(), cancellationToken: CancellationToken.None);

        response.Should().NotBeNull();
    }
}
