using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ProfitMaster;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitMaster;

public class BeneficiaryTests : ApiTestBase<Program>
{
    private readonly ScenarioFactory _scenarioFactory;
    private readonly IProfitMasterService _service;
    private const decimal BeneBalance = 9383m;

    public BeneficiaryTests()
    {
        // Arrange for Database
        Beneficiary beneficiary = StockFactory.CreateBeneficiary();
        ProfitDetail profitDetail = StockFactory.CreateAllocation(1901, beneficiary.Contact!.Ssn, BeneBalance);
        _scenarioFactory = new ScenarioFactory { Beneficiaries = [beneficiary], ProfitDetails = [profitDetail] };
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        
        // Arrange for fake user
        IAppUser appUser = new Mock<IAppUser>().Object; // Throws exception if we use the autowired one.
        
        IInternalProfitShareEditService internalProfitSharing = ServiceProvider?.GetRequiredService<IInternalProfitShareEditService>()!;
        _service = new ProfitMasterService(internalProfitSharing, MockDbContextFactory, appUser);
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
        int earnPointsExpected = (int)Math.Round(BeneBalance / 100, MidpointRounding.AwayFromZero);
        pd.Earnings.Should().Be(earnPointsExpected * req.EarningsPercent);
        pd.Forfeiture.Should().Be(0);
        pd.ProfitCode.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings);
        pd.ProfitCodeId.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings.Id);
    }
}
