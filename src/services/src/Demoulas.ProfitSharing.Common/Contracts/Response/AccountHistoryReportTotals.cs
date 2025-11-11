namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Cumulative totals for Account History Report
/// </summary>
public record AccountHistoryReportTotals
{
    public decimal TotalContributions { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalForfeitures { get; set; }
    public decimal TotalWithdrawals { get; set; }
}
