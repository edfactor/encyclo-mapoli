using Demoulas.Common.Contracts.Contracts.Response;
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
public class GetFrozenDemographicsEndpointTests : ApiTestBase<Program>
{
    [Fact]
    public async Task ExecuteAsync_ReturnsFrozenDemographics()
    {
        List<FrozenState> frozenDemographics = await MockDbContextFactory.UseReadOnlyContext(c => c.FrozenStates.ToListAsync());

        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);
        TestResult<PaginatedResponseDto<FrozenStateResponse>> response = await ApiClient.GETAsync<GetFrozenDemographicsEndpoint, PaginatedResponseDto<FrozenStateResponse>>();
        response.ShouldNotBeNull();

        // Assert each property

        var array = response.Result.Results.ToArray();
        for (int i = 0; i < frozenDemographics.Count; i++)
        {
            var expected = frozenDemographics[i];
            var actual = array[i];

            actual.AsOfDateTime.ShouldBe(expected.AsOfDateTime);
            actual.CreatedDateTime.ShouldBe(expected.CreatedDateTime);
            actual.ProfitYear.ShouldBe(expected.ProfitYear);
            actual.FrozenBy.ShouldBe(expected.FrozenBy);
            actual.IsActive.ShouldBe(expected.IsActive);
        }
    }
}
