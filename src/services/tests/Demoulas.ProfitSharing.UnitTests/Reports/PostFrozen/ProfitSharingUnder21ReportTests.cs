using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;

namespace Demoulas.ProfitSharing.UnitTests.Reports.PostFrozen;

public sealed class ProfitSharingUnder21ReportTests: ApiTestBase<Program>
{
    [Fact(DisplayName ="PS-419 Check Under 21 Report")]
    public async Task CheckUnder21Report()
    {
        var request = new ProfitYearRequest() { ProfitYear = 2024, Skip = 0, Take = 255 };
        var response = await ApiClient.GETAsync<ProfitSharingUnder21ReportEndpoint, ProfitYearRequest, ProfitSharingUnder21ReportResponse>(request);

        response.Should().NotBeNull();
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        response = await ApiClient.GETAsync<ProfitSharingUnder21ReportEndpoint, ProfitYearRequest, ProfitSharingUnder21ReportResponse>(request);

        response.Should().NotBeNull();
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

}
