using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ProfitShareUpdateServiceEndpointTests : ApiTestBase<Program>
{
    private readonly ProfitShareUpdateEndpoint _endpoint;
    private const short ProfitYear = 2024;

    public ProfitShareUpdateServiceEndpointTests()
    {
        IProfitShareUpdateService svc = ServiceProvider?.GetRequiredService<IProfitShareUpdateService>()!;
        _endpoint = new ProfitShareUpdateEndpoint(svc);
    }

    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = ProfitYear };

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitShareUpdateRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BasicQuery()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = ProfitYear };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitShareUpdateRequest, ProfitShareUpdateResponse>(req);

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
            ProfitShareUpdateRequest req = new() { ProfitYear = ProfitYear, AdjustContributionAmount = 20, MaxAllowedContributions = 1 };
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            TestResult<ProfitShareUpdateResponse> response =
                await ApiClient
                    .GETAsync<ProfitShareUpdateEndpoint,
                        ProfitShareUpdateRequest, ProfitShareUpdateResponse>(req);

            // Assert
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Result.HasExceededMaximumContributions.Should().BeTrue();
        });
    }


    private static async Task<PayProfit> EnsureEmployeeHasPoints(ProfitSharingDbContext c)
    {
        PayProfit pp = await c.PayProfits
            .Include(payProfit => payProfit.Demographic!)
            .ThenInclude(demographic => demographic.ContactInfo)
            .Include(p => p.Demographic != null)
            .FirstAsync(CancellationToken.None);
        pp.ProfitYear = ProfitYear;
        pp.PointsEarned = 100_000 / 100;
        await c.SaveChangesAsync(CancellationToken.None);
        return pp;
    }

#pragma warning disable AsyncFixer01
    [Fact]
    public async Task Verify_employee_contribution_earnings_and_incomingForfeiture()
    {
        // Arrange
        const int employeeIncome = 100_000;
        const int currentBalance = 33_000;
        const int badge = 77;
        _ = await SetupEmployee(badge, employeeIncome, currentBalance);
        ProfitShareUpdateRequest req = new()
        {
            ProfitYear = ProfitYear,
            ContributionPercent = 20,
            IncomingForfeitPercent = 1.1m,
            EarningsPercent = 12.7m,
            MaxAllowedContributions = 30_000
        };

        // ACT
        ProfitShareUpdateResponse response = await _endpoint.GetResponse(req, CancellationToken.None);

        // Assert
        int memberCount = response.Response.Results.Count();
        memberCount.Should().Be(1); // should be just 1 employee
        ProfitShareUpdateMemberResponse profitShareUpdateMember = response.Response.Results.First(mf => mf.Badge == badge);
        profitShareUpdateMember.Contributions.Should().Be(employeeIncome * 0.20m);
        profitShareUpdateMember.AllEarnings.Should().Be(currentBalance * 0.127m);
        profitShareUpdateMember.IncomingForfeitures.Should().Be(employeeIncome * .011m);
    }

    private async Task<int> SetupEmployee(int badge, decimal employeeIncome, decimal currentBalance)
    {
        return await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            const int ssn = 7;

            List<PayProfit> ppr = await ctx.PayProfits
                .Include(payProfit => payProfit.Demographic!)
                .ThenInclude(demographic => demographic.ContactInfo)
                .Include(p => p.Demographic != null)
                .ToListAsync(CancellationToken.None);

            // This knocks all the employees out of ProfitSharing
            foreach (PayProfit ppi in ppr)
            {
                ppi.ProfitYear = 3000;
                ppi.YearsInPlan = 0;
                ppi.EnrollmentId = Enrollment.Constants.NotEnrolled; /*0*/
            }
            // This knocks all the profit details out of the way
            foreach (var pdx in ctx.ProfitDetails)
            {
                pdx.ProfitYear = 3000;
            }

            // This knocks out all the bene's
            foreach (var b in ctx.Beneficiaries)
            {
                b.Amount = 0;
            }

            // Now we move 1 employee back in.
            Demographic demo = ppr[0].Demographic!;
            // They have 3 profit sharing rows.
            List<PayProfit> ppx = ppr.Where(p => p.Demographic == demo).ToList();

            // we take the first one and use it for THIS YEAR
            PayProfit pp0 = ppx[0];
            pp0.DemographicId.Should().Be(demo.Id);
            demo.Ssn = ssn;
            demo.EmployeeId = badge;
            pp0.ProfitYear = ProfitYear;
            pp0.PointsEarned = (long)(employeeIncome / 100);
            pp0.EnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions /*2*/;

            #if false
            Add Sencario.  new employee   has bene records from 10 years ago.   Ensure thier balance shows up.
            Add Secnario.  Overwrite the amount on the Beneficary table, ensure we are not using it as input
            #endif
            
            // for LAST YEAR.  - it is important to have a row, otherwise the vesting calculations joins across an empty row.
            PayProfit pp1 = ppx[1];
            pp1.DemographicId.Should().Be(demo.Id);
            pp1.ProfitYear = ProfitYear -1;
            pp1.YearsInPlan = 0; // This is important, otherwise we have no Totals
            
            // Setup some initial money so the earnings have a number to work with.
            var pd = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);
            pd.Ssn = 7;
            pd.ProfitYear = 2000;
            pd.ProfitYearIteration = 0;
            pd.DistributionSequence = 0;
            pd.ProfitCode = ProfitCode.Constants.IncomingContributions;
            pd.ProfitCodeId = 0;
            pd.Contribution = currentBalance;
            pd.Earnings = 0;
            pd.Forfeiture = 0;
            pd.MonthToDate = 1; // Drop this
            pd.YearToDate = 2000; // Drop this
            pd.Remark = null;
            pd.ZeroContributionReasonId = null;
            pd.ZeroContributionReason = null;
            pd.FederalTaxes = 0m;
            pd.StateTaxes = 0m;
            pd.TaxCode = null;
            pd.TaxCodeId = null;
            pd.CommentTypeId = null;
            pd.CommentType = null;
            pd.CommentRelatedCheckNumber = null;
            pd.CommentRelatedState = null;
            pd.CommentRelatedOracleHcmId = null;
            pd.CommentRelatedPsnSuffix = null;
            pd.CommentIsPartialTransaction = null;

            await ctx.SaveChangesAsync(CancellationToken.None);

            return ssn;
        });
    }
    
    [Fact]
    public async Task Verify_bene_gets_earnings()
    {
        // Arrange
        const decimal currentBalance = 10_000m;
        await SetupBeneficiary(currentBalance);

        ProfitShareUpdateRequest req = new()
        {
            ProfitYear = ProfitYear,
            ContributionPercent = 0,
            IncomingForfeitPercent = 0,
            EarningsPercent = 6.7m
        };

        // ACT
        ProfitShareUpdateResponse response = await _endpoint.GetResponse(req, CancellationToken.None);

        // Assert
        int memberCount = response.Response.Results.Count();
        memberCount.Should().Be(1); // should be just 1 bene
        ProfitShareUpdateMemberResponse profitShareUpdateMember = response.Response.Results.First();
        profitShareUpdateMember.Contributions.Should().Be(0);
        profitShareUpdateMember.AllEarnings.Should().Be(currentBalance*0.067m);
        profitShareUpdateMember.IncomingForfeitures.Should().Be(0);
    }

    private async Task SetupBeneficiary(decimal currentBalance)
    {
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            List<PayProfit> ppr = await ctx.PayProfits
                .Include(payProfit => payProfit.Demographic!)
                .ThenInclude(demographic => demographic.ContactInfo)
                .Include(p => p.Demographic != null)
                .ToListAsync(CancellationToken.None);

            // This knocks all the employees out of ProfitSharing
            foreach (PayProfit ppi in ppr)
            {
                ppi.ProfitYear = 3000;
                ppi.YearsInPlan = 0;
                ppi.EnrollmentId = Enrollment.Constants.NotEnrolled; /*0*/
            }

            // This knocks all the profit details out of the way
            foreach (var pdx in ctx.ProfitDetails)
            {
                pdx.ProfitYear = 3000;
            }

            // This knocks out all the bene's.   Why does this not have async nonsense?
            foreach (var bb in ctx.Beneficiaries)
            {
                bb.Amount = 0;
            }

            // Now we move 1 bene back in.
            Beneficiary b = await ctx.Beneficiaries.Include(b => b.Contact).FirstAsync(CancellationToken.None);
            b.Amount = currentBalance; // NOTE:::: This should not be used.
            b.Contact!.Ssn = 7;

            // NOTE::: This should be how the bene gets initial amount
            #if false
            var pd = await ctx.ProfitDetails.FirstAsync(CancellationToken.None);
            pd.Ssn = 7;
            pd.ProfitYear = 2000;
            pd.ProfitYearIteration = 0;
            pd.DistributionSequence = 0;
            pd.ProfitCode = ProfitCode.Constants.IncomingContributions;
            pd.ProfitCodeId = 6;
            pd.Contribution = currentBalance;
            pd.Earnings = 0;
            pd.Forfeiture = 0;
            pd.MonthToDate = 1; // Drop this
            pd.YearToDate = 2000; // Drop this
            pd.Remark = null;
            pd.ZeroContributionReasonId = null;
            pd.ZeroContributionReason = null;
            pd.FederalTaxes = 0m;
            pd.StateTaxes = 0m;
            pd.TaxCode = null;
            pd.TaxCodeId = null;
            pd.CommentTypeId = null;
            pd.CommentType = null;
            pd.CommentRelatedCheckNumber = null;
            pd.CommentRelatedState = null;
            pd.CommentRelatedOracleHcmId = null;
            pd.CommentRelatedPsnSuffix = null;
            pd.CommentIsPartialTransaction = null;
            #endif

            await ctx.SaveChangesAsync(CancellationToken.None);

        });
    }
}
