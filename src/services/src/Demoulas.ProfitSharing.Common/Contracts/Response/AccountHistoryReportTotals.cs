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
    public decimal TotalVestedBalance { get; set; }

    public static AccountHistoryReportTotals ResponseExample() => new()
    {
        TotalContributions = 50000.00m,
        TotalEarnings = 25000.00m,
        TotalForfeitures = 1000.00m,
        TotalWithdrawals = 10000.00m,
        TotalVestedBalance = 500000.00m
    };
}
