using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ProfitShareServiceEndpointTests : ApiTestBase<Program>
{
    private readonly ProfitShareUpdateEndpoint _endpoint;

    public ProfitShareServiceEndpointTests()
    {
        IProfitShareUpdateService svc = ServiceProvider?.GetRequiredService<IProfitShareUpdateService>()!;
        _endpoint = new ProfitShareUpdateEndpoint(svc);
    }

    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        ProfitSharingUpdateRequest req = new() { ProfitYear = 2024 };

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitSharingUpdateRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BasicQuery()
    {
        // Arrange
        ProfitSharingUpdateRequest req = new() { ProfitYear = 2024 };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitSharingUpdateRequest, ProfitShareUpdateResponse>(req);

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
            await EnsureEmployeeHasPoints(c);
            ProfitSharingUpdateRequest req = new()
            {
                ProfitYear = 2024, AdjustContributionAmount = 20, MaxAllowedContributions = 1
            };
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            TestResult<ProfitShareUpdateResponse> response =
                await ApiClient
                    .GETAsync<ProfitShareUpdateEndpoint,
                        ProfitSharingUpdateRequest, ProfitShareUpdateResponse>(req);

            // Assert
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Result.IsReRunRequired.Should().BeTrue();
        });
    }


    private static async Task<PayProfit> EnsureEmployeeHasPoints(ProfitSharingDbContext c)
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

#pragma warning disable AsyncFixer01
    [Fact]
    public async Task Ensure_contribution_is_correct()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arange
            List<PayProfit> ppr = await c.PayProfits
                .Include(payProfit => payProfit.Demographic!)
                .ThenInclude(demographic => demographic.ContactInfo)
                .Include(p => p.Demographic != null)
                .ToListAsync();
            // This knocks all the employees out of ProfitSharing
            foreach (PayProfit ppi in ppr)
            {
                ppi.YearsInPlan = 0;
                ppi.EnrollmentId = Enrollment.Constants.NotEnrolled; /*0*/
            }
            // Now we move 1 employee back in.
            Demographic demo = ppr[0].Demographic!;
            List<PayProfit> ppx = ppr.Where(p => p.Demographic == demo).ToList();
            PayProfit pp0 = ppx[0];
            demo.Ssn = int.MaxValue - 30_000;
            pp0.ProfitYear = 2024;
            pp0.PointsEarned = 1_000;
            pp0.EnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions /*2*/;
            await c.SaveChangesAsync();

            ProfitSharingUpdateRequest req = new() { ProfitYear = 2024, ContributionPercent = 20, MaxAllowedContributions = 30_000 };

            // ACT
            ProfitShareUpdateResponse response = await _endpoint.GetResponse(req, CancellationToken.None);

            // Assert
            MemberFinancialsResponse memberFinancials =
                response.Response.Results.First(mf => mf.Badge == demo.EmployeeId);
            memberFinancials.Contributions.Should().Be(20000);
        });
    }
}
