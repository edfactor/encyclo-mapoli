using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;
public class GetFrozenDemographicsEndpointTests : ApiTestBase<Program>
{
    [Fact]
    public async Task ExecuteAsync_ReturnsFrozenDemographics()
    {
        List<FrozenState> frozenDemographics = await MockDbContextFactory.UseReadOnlyContext(c => c.FrozenStates.ToListAsync());

        ApiClient.CreateAndAssignTokenForClient(Role.ITOPERATIONS);
        TestResult<PaginatedResponseDto<FrozenStateResponse>> response = await ApiClient.GETAsync<GetFrozenDemographicsEndpoint, PaginatedResponseDto<FrozenStateResponse>>();
        response.Should().NotBeNull();

        // Assert each property

        var array = response.Result.Results.ToArray();
        for (int i = 0; i < frozenDemographics.Count; i++)
        {
            var expected = frozenDemographics[i];
            var actual = array[i];

            actual.AsOfDateTime.ToUniversalTime().Should().BeSameDateAs(expected.AsOfDateTime.ToUniversalTime());
            actual.CreatedDateTime.ToUniversalTime().Should().BeSameDateAs(expected.CreatedDateTime.ToUniversalTime());
            actual.ProfitYear.Should().Be(expected.ProfitYear);
            actual.FrozenBy.Should().Be(expected.FrozenBy);
            actual.IsActive.Should().Be(expected.IsActive);
        }
    }
}
