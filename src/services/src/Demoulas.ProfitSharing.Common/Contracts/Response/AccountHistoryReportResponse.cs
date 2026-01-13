using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// DTO for Account History Report response, displaying member account activity by profit year.
/// Shows condensed yearly summaries of account balances and transactions.
/// </summary>
public sealed record AccountHistoryReportResponse : ProfitYearRequest
{
    public required int Id { get; set; }

    /// <summary>
    /// Employee badge number (identifier in payroll system).
    /// </summary>
    public int BadgeNumber { get; set; }

    /// <summary>
    /// Total contributions (employer) for the plan year.
    /// </summary>
    public decimal Contributions { get; set; }

    /// <summary>
    /// Total earnings (dividends) for the plan year.
    /// </summary>
    public decimal Earnings { get; set; }

    /// <summary>
    /// Total forfeitures for the plan year.
    /// </summary>
    public decimal Forfeitures { get; set; }

    /// <summary>
    /// Total withdrawals/allocations for the plan year.
    /// </summary>
    public decimal Withdrawals { get; set; }

    /// <summary>
    /// Ending balance at the end of the plan year (12/31).
    /// </summary>
    public decimal EndingBalance { get; set; }

    public static AccountHistoryReportResponse ResponseExample() => new()
    {
        Id = 1,
        BadgeNumber = 12345,
        ProfitYear = 2024,
        Contributions = 5000.00m,
        Earnings = 2500.00m,
        Forfeitures = 0m,
        Withdrawals = 1000.00m,
        EndingBalance = 125000.00m
    };
}
