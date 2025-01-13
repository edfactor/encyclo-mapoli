using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;

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
}
