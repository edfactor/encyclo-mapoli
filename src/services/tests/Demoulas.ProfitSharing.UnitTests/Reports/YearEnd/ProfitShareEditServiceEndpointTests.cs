using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[Collection("SharedGlobalState")]
public sealed class ProfitShareEditServiceEndpointTests : ApiTestBase<Program>
{
    private readonly short _profitYear = 2024;
    private readonly short _profitYearPast = 2020;
    private readonly decimal _beneInitialBalance = 8443m;

    public ProfitShareEditServiceEndpointTests()
    {
        // create mock database with just 1 bene
        var bene = StockFactory.CreateBeneficiary();
        var allocation = StockFactory.CreateAllocation(_profitYearPast, bene.Contact!.Ssn, _beneInitialBalance);
        MockDbContextFactory = new ScenarioFactory { Beneficiaries = [bene], ProfitDetails = [allocation] }.BuildMocks();
    }

    [Fact]
    public async Task Unauthorized()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear };

        // Act
        TestResult<ProfitShareEditResponse> response =
            await ApiClient
                .GETAsync<ProfitShareEditEndpoint,
                    ProfitShareUpdateRequest, ProfitShareEditResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task BasicQuery_json()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear, EarningsPercent = 7m };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<ProfitShareEditResponse> response =
            await ApiClient
                .GETAsync<ProfitShareEditEndpoint, ProfitShareUpdateRequest, ProfitShareEditResponse>(req);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    public static decimal ComputeBeneficiaryEarnings(decimal beneInitialBalance, decimal earningsPercent)
    {
        decimal pointsDollars = Math.Round(beneInitialBalance, 2, MidpointRounding.AwayFromZero);
        int earnPoints = (int)Math.Round(pointsDollars / 100, MidpointRounding.AwayFromZero);
        return Math.Round(earningsPercent * earnPoints, 2, MidpointRounding.AwayFromZero);
    }

    [Fact]
    public async Task BasicQuery_csv()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear, EarningsPercent = 8m };
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        TestResult<StreamContent> response = await DownloadClient.GETAsync<ProfitShareEditEndpoint, ProfitShareUpdateRequest, StreamContent>(req);

        string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();

        // Assert CSV format
        string csvData = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        string[] lines = csvData.Split(["\r\n", "\n"], StringSplitOptions.None);
        // line 0 is today's date
        int l = 0;
        lines[l++].ShouldNotBeEmpty(); // has Date/time
        lines[l++].ShouldBe("Profit Sharing Edit");
        lines[l].ShouldBe("Number,Name,Code,Contribution Amount,Earnings Amount,Incoming Forfeitures,Reason");
    }
}
