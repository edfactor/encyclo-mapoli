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

public class RevertEmployeeTests : ApiTestBase<Program>
{
    private readonly ScenarioFactory _scenarioFactory;
    private readonly List<ProfitDetail> _profitDetails;
    private readonly IProfitMasterService _service;
    private readonly short _thisYear;

    public RevertEmployeeTests()
    {
        // create mock database with just 1 employee with two profit detail rows in last year.
        _scenarioFactory = new ScenarioFactory().CreateOneEmployeeWithProfitDetails();
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        _profitDetails = _scenarioFactory.ProfitDetails;
        _thisYear = _scenarioFactory.ThisYear;

        _service = ServiceProvider?.GetRequiredService<IProfitMasterService>()!;
    }

    [Fact]
    public async Task ensure_employee_with_just_earnings_and_no_etva_is_reverted()
    {
        // Arrange
        var pd0 = _profitDetails[0];
        pd0.ProfitYear = _thisYear;
        pd0.ProfitCodeId = 0;

        // ignore this row    
        _profitDetails[1].ProfitYear = (short)(_thisYear - 5);

        // Act
        ProfitMasterResponse response = await _service.Revert(new ProfitYearRequest { ProfitYear = _thisYear }, CancellationToken.None);

        // Assert
        response.BeneficiariesEffected.Should().Be(0);
        response.EmployeesEffected.Should().Be(1);
        response.EtvasEffected.Should().Be(0);

        _scenarioFactory.removedProftitDetails.Count.Should().Be(1);
    }

    [Fact]
    public async Task ensure_employee_with_etva_is_reverted()
    {
        // Arrange
        var pd0 = _profitDetails[0];
        pd0.ProfitYear = _thisYear;
        pd0.ProfitCodeId = 0;

        // Etva should get unwound and removed from payprofit    
        var pd1 = _profitDetails[1];
        pd1.ProfitYear = _thisYear;
        pd1.ProfitCodeId = 8;
        pd1.Earnings = 100;
        pd1.Remark = /*"100% Earnings"*/ CommentType.Constants.OneHundredPercentEarnings.Name;
        pd1.CommentTypeId = CommentType.Constants.OneHundredPercentEarnings.Id;
        
        // ETVA is due to be added back real soon now.   https://demoulas.atlassian.net/browse/PS-630
        // _scenarioFactory.PayProfits[0].Etva = 700

        // Act
        ProfitMasterResponse response = await _service.Revert(new ProfitYearRequest { ProfitYear = _thisYear }, CancellationToken.None);

        // Assert
        response.BeneficiariesEffected.Should().Be(0);
        response.EmployeesEffected.Should().Be(1);
        response.EtvasEffected.Should().Be(1);

        _scenarioFactory.removedProftitDetails.Count.Should().Be(2);
    }
}
