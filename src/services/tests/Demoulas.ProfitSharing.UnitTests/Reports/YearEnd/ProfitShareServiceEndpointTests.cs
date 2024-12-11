using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using MassTransit.Courier;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ProfitShareServiceEndpointTests : ApiTestBase<Program>
{
    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        UpdateAdjustmentAmountsRequest req = new() { ProfitYear = 2024 };

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    UpdateAdjustmentAmountsRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BasicQuery()
    {
        // Arrange
        UpdateAdjustmentAmountsRequest req = new() { ProfitYear = 2024 };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    UpdateAdjustmentAmountsRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public void Ensure_max_contribution_is_tripped()
    {
        _ = MockDbContextFactory.UseWritableContext(async c =>
       {

           // Arrange
           // ensure we always have an employee with PointsEarned
           await ModifyAnEmployee(c);
           UpdateAdjustmentAmountsRequest req = new()
           {
               ProfitYear = 2024,
               AdjustContributionAmount = 20,
               MaxAllowedContributions = 1
           };
           ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

           // Act
           TestResult<ProfitShareUpdateResponse> response =
               await ApiClient
                   .GETAsync<ProfitShareUpdateEndpoint,
                       UpdateAdjustmentAmountsRequest, ProfitShareUpdateResponse>(req);

           // Assert
           response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
           response.Result.IsReRunRequired.Should().BeTrue();

       });
      
    }


    private static async Task<PayProfit> ModifyAnEmployee(ProfitSharingDbContext c)
    {
        PayProfit pp = await c.PayProfits
            .Include(payProfit => payProfit.Demographic!)
            .ThenInclude(demographic => demographic.ContactInfo)
            .Include(p => p.Demographic != null)
            .FirstAsync();
        Demographic demo = pp.Demographic!;

        pp.ProfitYear = 2024;
        pp.PointsEarned = 100_000 / 100;

        await c.SaveChangesAsync();
        return pp;
    }


}
