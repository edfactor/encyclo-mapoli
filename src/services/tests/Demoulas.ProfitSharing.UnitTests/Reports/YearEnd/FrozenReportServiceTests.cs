using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class FrozenReportServiceTests : ApiTestBase<Program>
{
    [Fact(DisplayName = "PS-61 - Get Forfeitures and Points report.  PAY443")]
    public async Task GetForfeituresAndPointsTests()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var request = new ProfitYearRequest() { ProfitYear = 2023, Skip = 0, Take = 255 };
        var response = await ApiClient.GETAsync<ForfeituresAndPointsForYearEndpoint, ProfitYearRequest, ForfeituresAndPointsForYearResponseWithTotals>(request);
        response.ShouldNotBeNull();
    }

    [Fact(DisplayName = "PS-61: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        var response =
            await ApiClient.GETAsync<ForfeituresAndPointsForYearEndpoint, ProfitYearRequest, ForfeituresAndPointsForYearResponseWithTotals>(new ProfitYearRequest());

        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

    }

    [Fact(DisplayName = "PS-404 - Gross Wages Report")]
    public async Task GetGrossWagesReport()
    {
        long demoSsn = 0;
        int demoBadgeNumber = 0;
        const short testYear = 2024;
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync(CancellationToken.None);
            demoSsn = demoTest.Ssn;
            demoBadgeNumber = demoTest.BadgeNumber;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync(CancellationToken.None);

            // Ensure we have at least 2 ProfitDetails records for the test
            pdArray.Length.ShouldBeGreaterThanOrEqualTo(2, "Test requires at least 2 ProfitDetails records");

            // Set up ProfitDetails for the test year (2024) and prior year (2023)
            pdArray[0].ProfitYear = testYear;
            pdArray[0].ProfitCode = ProfitCode.Constants.IncomingContributions;
            pdArray[0].ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            pdArray[0].Contribution = 1000m;
            pdArray[0].Earnings = 100m;
            pdArray[0].Forfeiture = 0m;
            pdArray[0].MonthToDate = 0;
            pdArray[0].YearToDate = testYear;
            pdArray[0].FederalTaxes = 0.5m;
            pdArray[0].StateTaxes = 0.25m;

            pdArray[1].ProfitYear = testYear - 1;
            pdArray[1].ProfitCode = ProfitCode.Constants.IncomingContributions;
            pdArray[1].ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id;
            pdArray[1].Contribution = 500m;
            pdArray[1].Earnings = 50m;
            pdArray[1].Forfeiture = 0m;
            pdArray[1].MonthToDate = 0;
            pdArray[1].YearToDate = testYear - 1;
            pdArray[1].FederalTaxes = 0.5m;
            pdArray[1].StateTaxes = 0.25m;

            // Set remaining records to years that won't interfere
            for (int i = 2; i < pdArray.Length; i++)
            {
                pdArray[i].ProfitYear = (short)(testYear - i);
            }

            var ppArray = await ctx.PayProfits.Where(x => x.DemographicId == demoTest.Id).ToArrayAsync(CancellationToken.None);
            // Ensure we have at least 2 PayProfits records
            ppArray.Length.ShouldBeGreaterThanOrEqualTo(2, "Test requires at least 2 PayProfits records");

            // Set all to a year that won't interfere first
            foreach (var pp in ppArray)
            {
                pp.ProfitYear = 10000;
            }
            // Set up for test year and prior year
            ppArray[0].ProfitYear = testYear;
            ppArray[0].IncomeExecutive = 25;
            ppArray[0].CurrentIncomeYear = 49995;
            ppArray[1].ProfitYear = testYear - 1;
            ppArray[1].IncomeExecutive = 0;
            ppArray[1].CurrentIncomeYear = 0;

            await ctx.SaveChangesAsync(CancellationToken.None);
        });

        var request = new GrossWagesReportRequest()
        {
            MinGrossAmount = 50000,
            ProfitYear = testYear,
            Skip = 0,
            Take = 1000
        };

        var response = await ApiClient.GETAsync<GrossWagesReportEndpoint, GrossWagesReportRequest, GrossWagesReportResponse>(request);
        response.ShouldNotBeNull();
        response.Result.Response.Total.ShouldBeGreaterThan(0);
        var testRec = response.Result.Response.Results.First(x => x.BadgeNumber == demoBadgeNumber);
        testRec.ShouldNotBeNull();
        testRec.GrossWages.ShouldBe(49995 + 25);
    }

    [Fact(DisplayName = "Update Summary Report - PS-394")]
    public async Task UpdateSummaryReportTests()
    {
        //Setup data
        int demoSsn = 0;
        int demoBadgeNumber = 0;

        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demoTest = await ctx.Demographics.FirstAsync(CancellationToken.None);
            demoSsn = demoTest.Ssn;
            demoBadgeNumber = demoTest.BadgeNumber;
            var pdArray = await ctx.ProfitDetails.Where(x => x.Ssn == demoTest.Ssn).ToArrayAsync(CancellationToken.None);

            foreach (var pd in pdArray)
            {
                pd.Contribution = 2500;
                pd.Earnings = 120;
                pd.Forfeiture = 0;
                pd.YearsOfServiceCredit = 1;
            }
        });

        //Check unauthorized
        var request = new ProfitYearRequest() { ProfitYear = 2023, Skip = 0, Take = 255 };
        var response = await ApiClient.GETAsync<UpdateSummaryReportEndpoint, ProfitYearRequest, UpdateSummaryReportResponse>(request);

        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        response = await ApiClient.GETAsync<UpdateSummaryReportEndpoint, ProfitYearRequest, UpdateSummaryReportResponse>(request);
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
    }
}
