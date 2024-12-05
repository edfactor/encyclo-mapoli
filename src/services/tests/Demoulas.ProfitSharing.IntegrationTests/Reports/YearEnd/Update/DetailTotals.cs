namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public record DetailTotals(
    decimal DistributionsTotal,
    decimal ForfeitsTotal,
    decimal AllocationsTotal,
    decimal PaidAllocationsTotal,
    decimal MilitaryTotal,
    decimal ClassActionFundTotal);
