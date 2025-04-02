using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitMasterService;

public class BeneficiaryTests : ApiTestBase<Program>
{
    private readonly Beneficiary _beneficiary;
    private readonly ScenarioFactory _scenarioFactory;
    private readonly IProfitMasterService _service;

    public BeneficiaryTests()
    {
        _beneficiary = StockFactory.CreateBeneficiary();
        _scenarioFactory = new ScenarioFactory { Beneficiaries = [_beneficiary] };
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        _service = ServiceProvider?.GetRequiredService<IProfitMasterService>()!;
    }

    [Fact]
    public async Task EnsureBeneficiaryHappyPath()
    {
        // Arrange
        ProfitShareUpdateRequest req = new() { ProfitYear = 2024, EarningsPercent = 11 };

        // Act
        ProfitMasterUpdateResponse updateResponse = await _service.Update(req, CancellationToken.None);

        // Assert
        // expect 1 record
        updateResponse.BeneficiariesEffected.Should().Be(1);
        updateResponse.EmployeesEffected.Should().Be(0);
        updateResponse.EtvasEffected.Should().Be(0);
        updateResponse.EarningsPercent.Should().Be(11);

        _scenarioFactory.addedProfitDetails.Count.Should().Be(1);
        ProfitDetail pd = _scenarioFactory.addedProfitDetails[0];
        pd.Contribution.Should().Be(0);
        int earnPointsExpected = (int)Math.Round(_beneficiary.Amount / 100, MidpointRounding.AwayFromZero);
        pd.Earnings.Should().Be(earnPointsExpected * req.EarningsPercent);
        pd.Forfeiture.Should().Be(0);
        pd.ProfitCode.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings);
        pd.ProfitCodeId.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings.Id);
    }
}
