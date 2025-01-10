using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitShareEdit;

public class BeneficiaryTests : ApiTestBase<Program>
{
    private readonly decimal _beneBalance = 777m;
    private readonly Beneficiary _beneficiary;
    private readonly IProfitShareEditService _service;

    public BeneficiaryTests()
    {
        // create mock database with just 1 bene
        _beneficiary = StockFactory.CreateBeneficiary();
        _beneficiary.Amount = _beneBalance;
        MockDbContextFactory = new ScenarioFactory { Beneficiaries = [_beneficiary] }.BuildMocks();

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
        ProfitShareEditMemberRecordResponse record = response.Response.Results.First();

        // compute expected 100 Earnings Amount
        decimal pointsDollars = Math.Round(_beneBalance, 2, MidpointRounding.AwayFromZero);
        int earnPoints = (int)Math.Round(pointsDollars / 100, MidpointRounding.AwayFromZero);
        // validate record
        record.Code.Should().Be( /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings);
        record.ContributionAmount.Should().Be(0);
        record.IncomingForfeitures.Should().Be(0);
        record.EarningsAmount.Should().Be(earnPoints * req.EarningsPercent);
        record.Reason.Should().Be(CommentType.Constants.OneHundredPercentEarnings.Name);
    }
}
