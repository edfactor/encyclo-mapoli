using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FastEndpoints;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public sealed class ProfitShareEditServiceEndpointTests : ApiTestBase<Program>
{
    private readonly short _profitYear = 2024;
    private readonly short _profitYearPast = 2020;
    private readonly decimal _beneInitialBalance = 8443m;
    private readonly short PsnSuffix = 721;
    private readonly int BadgeNumber = 1;

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
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task BasicQuery_json()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = _profitYear, EarningsPercent = 7m };
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        decimal earningsExpected = ComputeBeneficiaryEarnings(_beneInitialBalance, req.EarningsPercent);

        // Act
        TestResult<ProfitShareEditResponse> response =
            await ApiClient
                .GETAsync<ProfitShareEditEndpoint, ProfitShareUpdateRequest, ProfitShareEditResponse>(req);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);

        var r = response.Result;
        r.Response.Results.Should().ContainSingle();

        // Assert - that the bene is corect.
        var bene = r.Response.Results.First();
        bene.EarningsAmount.Should().Be(earningsExpected);
        bene.ContributionAmount.Should().Be(0);
        bene.ForfeitureAmount.Should().Be(0);
        bene.Code.Should().Be(8);
        bene.CommentTypeId.Should().Be(CommentType.Constants.OneHundredPercentEarnings.Id);
        bene.Remark.Should().Be(CommentType.Constants.OneHundredPercentEarnings.Name);
        bene.Psn.Should().Be($"{BadgeNumber * 10000 + PsnSuffix}");
        bene.BadgeNumber.Should().Be(0); // for Bene, we have PSN, so this is 0.
        bene.IsEmployee.Should().BeFalse();
        bene.YearExtension.Should().Be(0);

        // Assert - that the totals are correct.
        r.BeginningBalanceTotal.Should().Be(_beneInitialBalance);
        r.ContributionGrandTotal.Should().Be(0);
        r.EarningsGrandTotal.Should().Be(earningsExpected);
        r.IncomingForfeitureGrandTotal.Should().Be(0);
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
        decimal earningsExpected = ComputeBeneficiaryEarnings(_beneInitialBalance, req.EarningsPercent);

        // Act
        TestResult<StreamContent> response = await DownloadClient.GETAsync<ProfitShareEditEndpoint, ProfitShareUpdateRequest, StreamContent>(req);

        string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        result.Should().NotBeNullOrEmpty();

        // Assert CSV format
        string csvData = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        string[] lines = csvData.Split(["\r\n", "\n"], StringSplitOptions.None);
        // line 0 is today's date
        int l = 0;
        lines[l++].Should().NotBeEmpty(); // has Date/time
        lines[l++].Should().Be("Profit Sharing Edit");
        lines[l++].Should().Be("Number,Name,Code,Contribution Amount,Earnings Amount,Incoming Forfeitures,Reason");
        lines[l].Should().Be("10721,\"Benny, Ben\",8,0," + earningsExpected + ",0,100% Earnings");
    }
}
