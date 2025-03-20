using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Demographics;
public class GetFrozenDemographicsEndpointTests : ApiTestBase<Program>
{
    [Fact]
    public async Task ExecuteAsync_ReturnsFrozenDemographics()
    {
        List<FrozenState> frozenDemographics = await MockDbContextFactory.UseReadOnlyContext(c => c.FrozenStates.ToListAsync());

        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<PaginatedResponseDto<FrozenStateResponse>> response = await ApiClient.GETAsync<GetFrozenDemographicsEndpoint, PaginatedResponseDto<FrozenStateResponse>>();
        response.Should().NotBeNull();

        // Convert expected/actual values to UTC
        frozenDemographics.ForEach(d => d.AsOfDateTime = d.AsOfDateTime.ToUniversalTime());
        ((List<FrozenStateResponse>)response.Result.Results).ForEach(r => r.AsOfDateTime = r.AsOfDateTime.ToUniversalTime());

        // Assert
        Assert.Equivalent(frozenDemographics, response.Result.Results);
    }
}
