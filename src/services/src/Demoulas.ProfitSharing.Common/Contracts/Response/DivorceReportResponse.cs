using System;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// DTO for Divorce Report response, displaying member account activity by profit year.
/// Shows condensed yearly summaries for use in divorce proceedings.
/// </summary>
[NoMemberDataExposed]
public class DivorceReportResponse
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
    /// Plan year (calendar year) for this row of account activity.
    /// </summary>
    public int ProfitYear { get; set; }

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

    /// <summary>
    /// Optional comment or special notes for this record.
    /// </summary>
    public string? Comment { get; set; }
}
