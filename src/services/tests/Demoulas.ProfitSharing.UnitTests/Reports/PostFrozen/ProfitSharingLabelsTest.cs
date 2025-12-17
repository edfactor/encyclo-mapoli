using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.PostFrozen;

public sealed class ProfitSharingLabelsTest : ApiTestBase<Program>
{
    [Fact(DisplayName = "PS-764 - Profit Sharing labels")]
    public async Task CheckProfitSharingLabels()
    {
        var request = new ProfitYearRequest() { ProfitYear = 2024 };
        var response = await ApiClient.GETAsync<ProfitSharingLabelsEndpoint, ProfitYearRequest, PaginatedResponseDto<ProfitSharingLabelResponse>>(request);
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        response = await ApiClient.GETAsync<ProfitSharingLabelsEndpoint, ProfitYearRequest, PaginatedResponseDto<ProfitSharingLabelResponse>>(request);
        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    }
}
