namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
/// A single year of totals extracted from Profit Detail records.
/// </summary>
internal sealed record ProfitDetailTotals(
    decimal DistributionsTotal,
    decimal ForfeitsTotal,
    decimal AllocationsTotal,
    decimal PaidAllocationsTotal,
    decimal MilitaryTotal,
    decimal ClassActionFundTotal)
{
    public static ProfitDetailTotals Zero { get; set; } = new ProfitDetailTotals(0m, 0m, 0m, 0m, 0m, 0m);
}

