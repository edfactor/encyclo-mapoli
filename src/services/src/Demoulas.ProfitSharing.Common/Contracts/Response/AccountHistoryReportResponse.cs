using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Shared;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// DTO for Account History Report response, displaying member account activity by profit year.
/// Shows condensed yearly summaries of account balances and transactions.
/// </summary>
public sealed record AccountHistoryReportResponse : ProfitYearRequest, IFullNameProperty
{
    /// <summary>
    /// Employee badge number (identifier in payroll system).
    /// </summary>
    public int BadgeNumber { get; set; }

    /// <summary>
    /// Full name of the member.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Masked SSN for security (displays as XXX-XX-####).
    /// </summary>
    public string Ssn { get; set; } = string.Empty;

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
}
