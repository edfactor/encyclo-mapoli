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
}
