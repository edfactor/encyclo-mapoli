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

public class RevertBeneficiaryTests : ApiTestBase<Program>
{
    private readonly ScenarioFactory _scenarioFactory;
    private readonly IProfitMasterService _service;
    private readonly short _thisYear;

    public RevertBeneficiaryTests()
    {
        _scenarioFactory = new ScenarioFactory().CreateOneBeneWithProfitDetail();
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        _thisYear = _scenarioFactory.ThisYear;
        _service = ServiceProvider?.GetRequiredService<IProfitMasterService>()!;
    }

    [Fact]
    public async Task ensure_beneficiary_happy_path()
    {
        // Arrange
        var pd0 = _scenarioFactory.ProfitDetails[0];
        pd0.ProfitYear = _thisYear;
        pd0.ProfitCodeId = /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id;

        // Act
        ProfitMasterResponse response = await _service.Revert(new ProfitYearRequest { ProfitYear = _thisYear }, CancellationToken.None);

        // Assert
        response.BeneficiariesEffected.Should().Be(1);
        response.EmployeesEffected.Should().Be(0);
        response.EtvasEffected.Should().Be(0);

        _scenarioFactory.removedProftitDetails.Count.Should().Be(1);
    }
}
