using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
