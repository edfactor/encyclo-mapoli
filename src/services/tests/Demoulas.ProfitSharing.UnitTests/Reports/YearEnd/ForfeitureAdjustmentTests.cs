using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ForfeitureAdjustmentTests : ApiTestBase<Program>
{

    [Fact(DisplayName = "Get Forfeiture Adjustments - Success")]
    public async Task GetForfeitureAdjustmentsSuccessTest()
    {
        _ = await MockDbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync(CancellationToken.None);
            // Arrange
            ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
            var request = new SuggestedForfeitureAdjustmentRequest { Ssn = demoTest.Ssn };

            // Act
            var response = await ApiClient.GETAsync<GetForfeitureAdjustmentsEndpoint,
                SuggestedForfeitureAdjustmentRequest, SuggestedForfeitureAdjustmentResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);

            return true;
        });
    }

    [Fact(DisplayName = "Get Forfeiture Adjustments - Filter By SSN")]
    public async Task GetForfeitureAdjustmentsBySSNTest()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        _ = await MockDbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync(CancellationToken.None);
            var request = new SuggestedForfeitureAdjustmentRequest { Ssn = demoTest.Ssn, };

            // Act
            var response = await ApiClient.GETAsync<GetForfeitureAdjustmentsEndpoint,
                SuggestedForfeitureAdjustmentRequest, SuggestedForfeitureAdjustmentResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
            Assert.NotNull(response.Result);
            return true;
        });
    }

    [Fact(DisplayName = "Get Forfeiture Adjustments - Missing Required Role")]
    public async Task GetForfeitureAdjustmentsWithoutPermission()
    {
        // Arrange
        var request = new SuggestedForfeitureAdjustmentRequest { Badge = 3333 };

        // Act
        var response = await ApiClient.GETAsync<GetForfeitureAdjustmentsEndpoint,
            SuggestedForfeitureAdjustmentRequest, SuggestedForfeitureAdjustmentResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
    }
}
