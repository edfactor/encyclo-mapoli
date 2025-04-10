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

public class RevertEmployeeTests : ApiTestBase<Program>
{
    private readonly ScenarioFactory _scenarioFactory;
    private readonly List<ProfitDetail> _profitDetails;
    private readonly IProfitMasterService _service;
    private readonly short _profitYear;

    public RevertEmployeeTests()
    {
        // create mock database with just 1 employee with two profit detail rows in last year.
        _scenarioFactory = new ScenarioFactory().CreateOneEmployeeWithProfitDetails().WithYearEndStatuses();
        MockDbContextFactory = _scenarioFactory.BuildMocks();
        _profitDetails = _scenarioFactory.ProfitDetails;
        _profitYear = _scenarioFactory.ProfitYear;

        // Arrange for fake user
        IAppUser appUser = new Mock<IAppUser>().Object; // Throws exception if we use the autowired one.
        
        IInternalProfitShareEditService internalProfitSharing = ServiceProvider?.GetRequiredService<IInternalProfitShareEditService>()!;
        _service = new ProfitMasterService(internalProfitSharing, MockDbContextFactory, appUser);
    }

    [Fact]
    public async Task ensure_employee_with_just_earnings_and_no_etva_is_reverted()
    {
        // Arrange
        var pd0 = _profitDetails[0];
        pd0.ProfitYear = _profitYear;
        pd0.ProfitCodeId = 0;
        pd0.Contribution = 1000m;

        // ignore this row (provided by the test setup)   
        _profitDetails[1].ProfitYear = (short)(_profitYear - 5);

        // Act
        ProfitMasterRevertResponse response = await _service.Revert(new ProfitYearRequest { ProfitYear = _profitYear }, CancellationToken.None);

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

        // Create contribution of 1000
        var pd0 = _profitDetails[0];
        pd0.ProfitYear = _profitYear;
        pd0.ProfitCodeId = 0;
        pd0.Contribution = 1000m;

        // create a 100% Earnings amount of 100
        var pd1 = _profitDetails[1];
        pd1.ProfitYear = _profitYear;
        pd1.ProfitCodeId = 8;
        pd1.Earnings = 100;
        pd1.Remark = /*"100% Earnings"*/ CommentType.Constants.OneHundredPercentEarnings.Name;
        pd1.CommentTypeId = CommentType.Constants.OneHundredPercentEarnings.Id;

        //  The two ETVA should be identical, but only ppNow is hot.
        var ppProfitYear = _scenarioFactory.PayProfits[0];
        ppProfitYear.ProfitYear = _profitYear;
        ppProfitYear.Etva = (decimal)new Random().NextDouble(); // emphasize that this field doest matter

        // Setup 177 as the ETVA, which means the 100 (from the 100% earnings) was just added to the ETVA
        var ppNow = _scenarioFactory.PayProfits[1];
        ppNow.ProfitYear = (short)(_profitYear + 1);
        ppNow.Etva = 177;

        // Act - lets do the revert (finally)
        ProfitMasterRevertResponse response = await _service.Revert(new ProfitYearRequest { ProfitYear = _profitYear }, CancellationToken.None);

        // Assert - we only had 1 person, so duh these should only be 1 person.
        response.BeneficiariesEffected.Should().Be(0);
        response.EmployeesEffected.Should().Be(1);
        response.EtvasEffected.Should().Be(1);

        // Assert that we set zero into last years PayProfit.  This is the limbo value because when you unwind,  
        ppProfitYear.Etva.Should().Be(0);
        
        // Finally the 177 should have the 100 removed.
        ppNow.Etva.Should().Be(77);

        _scenarioFactory.removedProftitDetails.Count.Should().Be(2);
    }
}
