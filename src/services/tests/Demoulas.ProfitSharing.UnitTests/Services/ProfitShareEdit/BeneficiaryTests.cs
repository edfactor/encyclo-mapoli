using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitShareEdit;

public class BeneficiaryTests : ApiTestBase<Program>
{
    private readonly Beneficiary _beneficiary;
    private readonly decimal _beneficiaryBalance = 1_000_000;
    private readonly ProfitDetail _profitDetail;
    private readonly IProfitShareEditService _service;

    public BeneficiaryTests()
    {
        // create mock database with just 1 bene
        _beneficiary = StockFactory.CreateBeneficiary();
        _profitDetail = StockFactory.CreateAllocation(1901, _beneficiary.Contact!.Ssn, _beneficiaryBalance);
        MockDbContextFactory = new ScenarioFactory { Beneficiaries = [_beneficiary], ProfitDetails = [_profitDetail]}.BuildMocks();

        _service = ServiceProvider?.GetRequiredService<IProfitShareEditService>()!;
    }

    [Fact]
    public async Task EnsureBeneficiaryHappyPath()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = 2024, EarningsPercent = 11 };

        // Act
        ProfitShareEditResponse response = await _service.ProfitShareEdit(req, CancellationToken.None);

        // Assert
        // expect 1 record
        response.Response.Results.Count().Should().Be(1);
        var record = response.Response.Results.First();

        // compute expected 100 Earnings Amount
        decimal expectedEarnings = ProfitShareEditServiceEndpointTests.ComputeBeneficiaryEarnings(_beneficiaryBalance, req.EarningsPercent);
        // validate record
        record.Code.Should().Be( /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings);
        record.ContributionAmount.Should().Be(0);
        record.ForfeitureAmount.Should().Be(0);
        record.EarningsAmount.Should().Be(expectedEarnings);
        record.Remark.Should().Be(CommentType.Constants.OneHundredPercentEarnings.Name);
    }
}
