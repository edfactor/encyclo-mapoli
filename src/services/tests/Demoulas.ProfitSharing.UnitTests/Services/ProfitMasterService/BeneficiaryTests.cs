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
    private readonly IProfitMasterService _service;
    private readonly ScenarioFactory _scenarioFactory;

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
        ProfitMasterResponse response = await _service.Update(req, CancellationToken.None);

        // Assert
        // expect 1 record
        response.BeneficiariesEffected.Should().Be(1);
        response.EmployeesEffected.Should().Be(0);
        response.EtvasEffected.Should().Be(0);
        
        _scenarioFactory.addedProfitDetails.Count.Should().Be(1);
        var pd = _scenarioFactory.addedProfitDetails[0];
        pd.Contribution.Should().Be(0);
        var earnPointsExpected = (int)Math.Round( _beneficiary.Amount / 100, MidpointRounding.AwayFromZero);
        pd.Earnings.Should().Be(earnPointsExpected * req.EarningsPercent);
        pd.Forfeiture.Should().Be(0);
        pd.ProfitCode.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings);
        pd.ProfitCodeId.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings.Id);
    }
}
