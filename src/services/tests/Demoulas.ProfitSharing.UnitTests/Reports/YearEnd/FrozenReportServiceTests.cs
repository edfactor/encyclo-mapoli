using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class FrozenReportServiceTests : ApiTestBase<Program>
{
   [Fact(DisplayName ="PS-61 - Get Forfeitures and Points report.  PAY443")]
    public async Task GetForfeituresAndPointsTests()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new ProfitYearRequest() { ProfitYear = 2023, Skip = 0, Take = 255 };
        var response = await ApiClient.GETAsync<ForfeituresAndPointsForYearEndpoint, ProfitYearRequest, ReportResponseBase< ForfeituresAndPointsForYearResponse>> (request);
        response.Should().NotBeNull();
    }

    [Fact(DisplayName = "PS-61: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        var response =
            await ApiClient.GETAsync<ForfeituresAndPointsForYearEndpoint, ProfitYearRequest, ReportResponseBase<ForfeituresAndPointsForYearResponse>>(new ProfitYearRequest());

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

    }

    [Fact(DisplayName = "PS-452 - Get Forfeitures and Points report with frozen data.  PAY443")]
    public async Task GetForfeituresAndPointsTestsWithFrozenData()
    {
        var testValue = 5714;
        Demographic? demographic = null;
        var newDob = new DateOnly(1991, 09, 21);
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            short sampleYear = 2023;
            demographic = await ctx.Demographics.FirstAsync();
            var profitDetails = await ctx.ProfitDetails.Where(x => x.Ssn == demographic.Ssn).ToListAsync();

            demographic.EmploymentStatusId = 'a';

            profitDetails.ForEach(pd => pd.ProfitYear = 0);
            profitDetails[0].ProfitYear = sampleYear;
            profitDetails[0].ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
            profitDetails[0].Forfeiture = testValue;

        });

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new FrozenProfitYearRequest() { ProfitYear = 2023, Skip = 0, Take = int.MaxValue, UseFrozenData = false };
        var response = await ApiClient.GETAsync<ForfeituresAndPointsForYearEndpoint, FrozenProfitYearRequest, ReportResponseBase<ForfeituresAndPointsForYearResponse>>(request);
        response.Should().NotBeNull();
        response.Result.Response.Total.Should().BeGreaterThan(0);
    }

    [Fact(DisplayName = "PS-404 - Gross Wages Report")]
    public async Task GetGrossWagesReport()
    {
        long demoSsn = 0;
        int demoBadgeNumber = 0;
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        _ = MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync(CancellationToken.None);
            demoSsn = demoTest.Ssn;
            demoBadgeNumber = demoTest.BadgeNumber;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync(CancellationToken.None);

            for (int i = 0; i < pdArray.Length; i++)
            {
                var prof = pdArray[i];
                prof.ProfitYear = (short)(DateTime.Now.Year - i);
                prof.ProfitCode = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal;
                prof.ProfitCodeId = ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id;
                prof.Contribution = Convert.ToDecimal(Math.Pow(2, i * 3));
                prof.Earnings = Convert.ToDecimal(Math.Pow(2, i * 3 + 1));
                prof.Forfeiture = Convert.ToDecimal(Math.Pow(2, i * 3 + 2));
                prof.MonthToDate = 0;
                prof.YearToDate = (short)(DateTime.Now.Year - i);
                prof.FederalTaxes = 0.5m;
                prof.StateTaxes = 0.25m;
            }

            var ppArray = await ctx.PayProfits.Where(x=>x.DemographicId== demoTest.Id).ToArrayAsync(CancellationToken.None);
            foreach (var pp in ppArray)
            {
                pp.ProfitYear = 10000;
            }
            ppArray[0].ProfitYear = 2023;
            ppArray[0].IncomeExecutive = 25;
            ppArray[0].CurrentIncomeYear = 0;
            ppArray[1].ProfitYear = 2022;
            ppArray[1].IncomeExecutive = 0;
            ppArray[1].CurrentIncomeYear = 49995;

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        var request = new GrossWagesReportRequest()
        {
            MinGrossAmount = 50000,
            ProfitYear = 2023,
            Skip = 0,
            Take = 1000
        };

        var response = await ApiClient.GETAsync<GrossWagesReportEndpoint, GrossWagesReportRequest,  GrossWagesReportResponse>(request);
        response.Should().NotBeNull();
        response.Result.Response.Total.Should().BeGreaterThan(0);
        var testRec = response.Result.Response.Results.First(x => x.BadgeNumber == demoBadgeNumber);
        testRec.Should().NotBeNull();
        testRec.GrossWages.Should().Be(49995 + 25);
    }

    [Fact(DisplayName = "Update Summary Report - PS-394")]
    public async Task UpdateSummaryReportTests()
    {
        //Check unauthorized
        var request = new ProfitYearRequest() { ProfitYear = 2023, Skip = 0, Take = 255 };
        var response = await ApiClient.GETAsync<UpdateSummaryReportEndpoint, ProfitYearRequest, UpdateSummaryReportResponse>(request);

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        response = await ApiClient.GETAsync<UpdateSummaryReportEndpoint, ProfitYearRequest, UpdateSummaryReportResponse>(request);
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Result.Should().NotBeNull();
        response.Result.TotalAfterProfitSharingAmount.Should().Be(10849.41m);
    }
}
