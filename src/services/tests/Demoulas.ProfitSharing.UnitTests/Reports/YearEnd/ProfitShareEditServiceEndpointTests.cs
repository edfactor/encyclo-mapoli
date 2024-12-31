using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using Demoulas.ProfitSharing.UnitTests.Mocks;
using FastEndpoints;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public sealed class ProfitShareEditServiceEndpointTests : ApiTestBase<Program>
{
    private readonly short _profitYear = 2024;

    public ProfitShareEditServiceEndpointTests()
    {
        // create mock database with just 1 bene
        MockDbContextFactory = new ScenarioFactory { Beneficiaries = [StockFactory.CreateBeneficiary()] }.BuildMocks();
    }

    [Fact]
    public async Task Unauthorized()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear };

        // Act
        TestResult<ProfitShareUpdateResponse> response =
            await ApiClient
                .GETAsync<ProfitShareUpdateEndpoint,
                    ProfitShareUpdateRequest, ProfitShareUpdateResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task BasicQuery_json()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<ProfitShareEditResponse> response =
            await ApiClient
                .GETAsync<ProfitShareEditEndpoint, ProfitShareUpdateRequest, ProfitShareEditResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Result.BeginningBalance.Should().BeGreaterThan(0);
        response.Result.ContributionGrandTotal.Should().BeGreaterThan(0);
        response.Result.EarningsGrandTotal.Should().BeGreaterThan(0);
        response.Result.IncomingForfeitureGrandTotal.Should().BeGreaterThan(0);

        response.Result.Response.Results.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task BasicQuery_csv()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear };
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<StreamContent> response = await DownloadClient.GETAsync<ProfitShareEditEndpoint, ProfitShareUpdateRequest, StreamContent>(req);

        string result = await response.Response.Content.ReadAsStringAsync();
        result.Should().NotBeNullOrEmpty();

        // Assert CSV format
        string csvData = await response.Response.Content.ReadAsStringAsync();
        string[] lines = csvData.Split(["\r\n", "\n"], StringSplitOptions.None);
        // line 0 is today's date
        int l = 0;
        lines[l++].Should().NotBeEmpty(); // has Date/time
        lines[l++].Should().Be("Profit Sharing Edit");
        lines[l++].Should().Be("Number,Name,Code,Contribution Amount,Earnings Amount,Incoming Forfeitures,Reason");
        lines[l].Should().Be("10721,\"Benny, Ben\",0,0,0,0,TBD");
    }
}
