using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

public class GetActiveFrozenDemographicEndpointTests : ApiTestBase<Program>
{
    [Fact]
    public async Task ExecuteAsync_ReturnsActiveFrozenDemographic()
    {
       
        FrozenState frozenDemographics = await MockDbContextFactory.UseReadOnlyContext(c => c.FrozenStates.Where(f=> f.IsActive).FirstAsync());

        ApiClient.CreateAndAssignTokenForClient(Role.ITOPERATIONS);
        TestResult<FrozenStateResponse> response = await ApiClient.GETAsync<GetActiveFrozenDemographicEndpoint, FrozenStateResponse>();
        response.Should().NotBeNull();


        response.Result.AsOfDateTime.ToUniversalTime().Should().BeSameDateAs(frozenDemographics.AsOfDateTime.ToUniversalTime());
        response.Result.CreatedDateTime.ToUniversalTime().Should().BeSameDateAs(frozenDemographics.CreatedDateTime.ToUniversalTime());
        response.Result.ProfitYear.Should().Be(frozenDemographics.ProfitYear);
        response.Result.FrozenBy.Should().Be(frozenDemographics.FrozenBy);
        response.Result.IsActive.Should().Be(frozenDemographics.IsActive);
    }
}
