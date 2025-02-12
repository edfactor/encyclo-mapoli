using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Demographics;

public class GetActiveFrozenDemographicEndpointTests : ApiTestBase<Program>
{
    [Fact]
    public async Task ExecuteAsync_ReturnsActiveFrozenDemographic()
    {
       
        FrozenState frozenDemographics = await MockDbContextFactory.UseReadOnlyContext(c => c.FrozenStates.Where(f=> f.IsActive).FirstAsync());

        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<FrozenStateResponse> response = await ApiClient.GETAsync<GetActiveFrozenDemographicEndpoint, FrozenStateResponse>();
        response.Should().NotBeNull();

        
        frozenDemographics.AsOfDateTime = frozenDemographics.AsOfDateTime.ToUniversalTime();
        response.Result.AsOfDateTime = frozenDemographics.AsOfDateTime.ToUniversalTime();

        // Assert
        frozenDemographics.Should().BeEquivalentTo(response.Result);
    }
}
