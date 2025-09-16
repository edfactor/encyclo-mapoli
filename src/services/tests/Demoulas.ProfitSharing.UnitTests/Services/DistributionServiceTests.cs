using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Shouldly;
using FastEndpoints;
using Demoulas.Common.Contracts.Contracts.Response;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Services;
public sealed class DistributionServiceTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "Distribution Search")]
    public async Task SearchAsync_ShouldReturnExpectedResults()
    {
        Distribution? firstDist = null;
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            firstDist = await ctx.Distributions.FirstOrDefaultAsync();
            return true;
        });

        var request = new DistributionSearchRequest
        {
            Take = 25,
            Skip = 0,
        };

        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);
        var response = await ApiClient.POSTAsync<
            DistributionSearchEndpoint, 
            DistributionSearchRequest, 
            PaginatedResponseDto<DistributionSearchResponse>>(request);

        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
        response.Result.Total.ShouldBeGreaterThan(0);
        response.Result.Results.Count().ShouldBeGreaterThan(0);
    }
}
