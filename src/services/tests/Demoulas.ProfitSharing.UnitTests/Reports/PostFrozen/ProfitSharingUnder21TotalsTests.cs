using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Reports.PostFrozen;

[CollectionDefinition("PS-759-Under21Totals", DisableParallelization = true)]
public sealed class ProfitSharingUnder21TotalsTests : ApiTestBase<Program>
{
    public ProfitSharingUnder21TotalsTests()
    {
    }

    /// <summary>
    ///   This endpoint is computationally expensive with large mock datasets.
    ///   Increase timeout to allow for legitimate slowness in business logic.
    /// </summary>
    protected override TimeSpan? GetHttpClientTimeout() => TimeSpan.FromMinutes(2);

    [Fact(DisplayName = "PS-759 - Profit Sharing under 21 ProfitShareUpdateTotals")]
    public async Task CheckUnder21Totals()
    {
        var request = new ProfitYearRequest() { ProfitYear = 2024, Skip = 0, Take = 255 };

        var response = await ApiClient.GETAsync<ProfitSharingUnder21TotalsEndpoint, ProfitYearRequest, ProfitSharingUnder21TotalsResponse>(request);

        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        response = await ApiClient.GETAsync<ProfitSharingUnder21TotalsEndpoint, ProfitYearRequest, ProfitSharingUnder21TotalsResponse>(request);

        response.ShouldNotBeNull();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
