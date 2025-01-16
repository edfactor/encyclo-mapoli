using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable S1199

namespace Demoulas.ProfitSharing.UnitTests.Services.ProfitMasterService;

public class UpdateEmployeeTests : ApiTestBase<Program>
{
    private const decimal DefaultEarningsPercent = 5;

    private readonly ScenarioFactory _scenarioFactory;
    private readonly List<ProfitDetail> _profitDetails;
    private readonly IProfitMasterService _service;
    private readonly short _thisYear;

    public UpdateEmployeeTests()
    {
        // create mock database with just 1 employee with two profit detail rows in last year.
        _scenarioFactory = new ScenarioFactory().CreateOneEmployeeWithProfitDetails();
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        _profitDetails = _scenarioFactory.ProfitDetails;
        _thisYear = _scenarioFactory.ThisYear;

        _service = ServiceProvider?.GetRequiredService<IProfitMasterService>()!;
    }

    [Fact]
    public async Task ensure_happy_path_works_fine()
    {
        // Arrange

        // Act
        MasterUpdateResponse response = await _service.Update(DefaultRequest(), CancellationToken.None);

        // Assert
        response.BeneficiariesEffected.Should().Be(0);
        response.EmployeesEffected.Should().Be(1);
        response.EtvasUpdated.Should().Be(0);

        _scenarioFactory.addedProfitDetails.Count.Should().Be(1);
        var pd = _scenarioFactory.addedProfitDetails[0];
        pd.Contribution.Should().Be(0);
        var earnPoints = (int)Math.Round(1000m / 100, MidpointRounding.AwayFromZero);
        pd.Earnings.Should().Be(earnPoints * DefaultEarningsPercent);
        pd.Forfeiture.Should().Be(0);
        pd.ProfitCode.Should().Be(ProfitCode.Constants.IncomingContributions);
    }

    [Fact]
    public async Task employee_with_Etva()
    {
        // Arrange
        _profitDetails[0].ProfitCodeId.Should().Be(0);
        _profitDetails[0].Contribution = 2000m;
        _profitDetails[1].ProfitCodeId.Should().Be(8);
        _profitDetails[1].Earnings = 1000m; // Etva Amount

        // Act
        MasterUpdateResponse response = await _service.Update(DefaultRequest(), CancellationToken.None);

        // Assert
        response.BeneficiariesEffected.Should().Be(0);
        response.EmployeesEffected.Should().Be(1);
        response.EtvasUpdated.Should().Be(1);

        _scenarioFactory.addedProfitDetails.Count.Should().Be(2);
        {
            var pd0 = _scenarioFactory.addedProfitDetails[0];
            pd0.ProfitCodeId.Should().Be( /*0*/ProfitCode.Constants.Incoming100PercentVestedEarnings.Id);
            pd0.ProfitCode.Should().Be(ProfitCode.Constants.Incoming100PercentVestedEarnings);
            pd0.Contribution.Should().Be(0);
            pd0.Forfeiture.Should().Be(0);
            var ETVAEarnPoints = (int)Math.Round(1000m / 100, MidpointRounding.AwayFromZero);
            pd0.Earnings.Should().Be(ETVAEarnPoints * DefaultEarningsPercent);
        }
        {
            var pd1 = _scenarioFactory.addedProfitDetails[1];
            pd1.ProfitCodeId.Should().Be(/*0*/ProfitCode.Constants.IncomingContributions.Id);
            pd1.ProfitCode.Should().Be(ProfitCode.Constants.IncomingContributions);
            pd1.Contribution.Should().Be(0);
            pd1.Forfeiture.Should().Be(0);
            var EarnPoints = (int)Math.Round(2000m / 100, MidpointRounding.AwayFromZero);
            pd1.Earnings.Should().Be(EarnPoints * DefaultEarningsPercent);
        }
    }


    private ProfitShareUpdateRequest DefaultRequest()
    {
        return new ProfitShareUpdateRequest { ProfitYear = _thisYear, EarningsPercent = DefaultEarningsPercent };
    }
}
