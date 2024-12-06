namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

/// <summary>
/// Totals extracted from Profit Detail records
/// </summary>
public record DetailTotals(
    decimal DistributionsTotal,
    decimal ForfeitsTotal,
    decimal AllocationsTotal,
    decimal PaidAllocationsTotal,
    decimal MilitaryTotal,
    decimal ClassActionFundTotal);
