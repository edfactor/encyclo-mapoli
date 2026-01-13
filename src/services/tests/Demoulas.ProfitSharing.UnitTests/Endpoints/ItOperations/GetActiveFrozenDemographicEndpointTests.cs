using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

[Collection("IT Operations Tests")]
public class GetActiveFrozenDemographicEndpointTests : ApiTestBase<Program>
{
    [Fact]
    public async Task ExecuteAsync_ReturnsActiveFrozenDemographic()
    {

        FrozenState frozenDemographics = await MockDbContextFactory.UseReadOnlyContext(c => c.FrozenStates.Where(f => f.IsActive).FirstAsync());

        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);
        TestResult<FrozenStateResponse> response = await ApiClient.GETAsync<GetActiveFrozenDemographicEndpoint, FrozenStateResponse>();
        response.ShouldNotBeNull();


        response.Result.AsOfDateTime.ToUniversalTime().ShouldBe(frozenDemographics.AsOfDateTime.ToUniversalTime());
        response.Result.CreatedDateTime.ToUniversalTime().ShouldBe(frozenDemographics.CreatedDateTime.ToUniversalTime());
        response.Result.ProfitYear.ShouldBe(frozenDemographics.ProfitYear);
        response.Result.FrozenBy.ShouldBe(frozenDemographics.FrozenBy);
        response.Result.IsActive.ShouldBe(frozenDemographics.IsActive);
    }
}
