using System.Net;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.PostFrozen;

public class ProfitSharingUnder21BreakdownByStoreTests : ApiTestBase<Program>
{
    [Fact(DisplayName = "PS-759 - Profit Sharing under 21 Breakdown by Store")]
    public async Task CheckUnder21BreakdownByStoreReport()
    {
        var request = new ProfitYearRequest() { ProfitYear = 2024, Skip = 0, Take = 255 };
        var response = await ApiClient.GETAsync<ProfitSharingUnder21BreakdownByStoreEndpoint, ProfitYearRequest, PaginatedResponseDto<ProfitSharingUnder21BreakdownByStoreResponse>>(request);

        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        response = await ApiClient.GETAsync<ProfitSharingUnder21BreakdownByStoreEndpoint, ProfitYearRequest, PaginatedResponseDto<ProfitSharingUnder21BreakdownByStoreResponse>>(request);
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);

    }
}
