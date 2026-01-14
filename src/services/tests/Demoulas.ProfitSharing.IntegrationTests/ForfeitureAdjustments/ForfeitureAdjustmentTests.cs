using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Demoulas.ProfitSharing.Services;
using Moq;
using Shouldly;

#pragma warning disable VSTHRD002 // Synchronous waits are safe in xUnit tests

namespace Demoulas.ProfitSharing.IntegrationTests.ForfeitureAdjustments;

public class ForfeitureAdjustmentTests : PristineBaseTest
{
    public ForfeitureAdjustmentTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void ensure_forfeiture_adjustment_suggestions_respect_etva()
    {
        // Badge 700655: Terminated in 2024, has Etva amount
        // Zero because their ETVA = Current Amount and/or ETVA > Vested Amount
        // ExpectBadgeShouldHaveSuggested(700655, 0)

        // problem badges pulled from READY, these members all have ETVA amounts 
        ExpectBadgeShouldHaveSuggested(703625, 3893.48m);
        // ExpectBadgeShouldHaveSuggested(702791, 12743.74m)
        // ExpectBadgeShouldHaveSuggested(704643, 2717.16m)
    }

    [Fact]
    public async Task ensure_forfeiture_adjustment_suggestions_work_without_etva()
    {
        const int badge = 703625; // Terminated in 2024, has no ETVA
        var employee = await GetEmployeeByBadgeAsync(badge);
        employee.PayProfit.Etva.ShouldBe(10.10m);

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IAuditService>();
        ForfeitureAdjustmentService fas = new ForfeitureAdjustmentService(DbFactory, TotalService, DemographicReaderService, TimeProvider, mockAppUser.Object, mockAuditService.Object);
        SuggestedForfeitureAdjustmentRequest sfar = new() { Badge = badge };
        var res = (await fas.GetSuggestedForfeitureAmount(sfar)).Value!;

        res.SuggestedForfeitAmount.ShouldBe(.23m);
    }

    private void ExpectBadgeShouldHaveSuggested(int badge, decimal suggestedForfeitAmount)
    {
        var employee = GetEmployeeByBadgeAsync(badge).GetAwaiter().GetResult();
        employee.PayProfit.Etva.ShouldNotBe(0m);

        var mockAppUser = new Mock<IAppUser>();
        var mockAuditService = new Mock<IAuditService>();
        ForfeitureAdjustmentService fas = new ForfeitureAdjustmentService(DbFactory, TotalService, DemographicReaderService, TimeProvider, mockAppUser.Object, mockAuditService.Object);
        SuggestedForfeitureAdjustmentRequest sfar = new() { Badge = badge };
        var res = fas.GetSuggestedForfeitureAmount(sfar).GetAwaiter().GetResult().Value!;

        res.SuggestedForfeitAmount.ShouldBe(suggestedForfeitAmount);
    }
}
