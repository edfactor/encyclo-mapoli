using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.PostFrozen;

public class ProfitControlSheetTests : ApiTestBase<Program>
{
    [Fact(DisplayName = "PS-897 - Control Sheet")]
    public async Task CheckProfitControlSheets()
    {
        var request = new ProfitYearRequest() { ProfitYear = 2024 };
        var response = await ApiClient.GETAsync<ProfitControlSheetEndpoint, ProfitYearRequest, ProfitControlSheetResponse>(request);
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        response = await ApiClient.GETAsync<ProfitControlSheetEndpoint, ProfitYearRequest, ProfitControlSheetResponse>(request);
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
