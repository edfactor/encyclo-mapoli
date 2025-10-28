using System;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// DTO for Divorce Report response, displaying member account activity condensed by profit year.
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
    /// Total contributions (employer) for the plan year in cents.
    /// </summary>
    public long TotalContributions { get; set; }

    /// <summary>
    /// Total withdrawals/loans for the plan year in cents.
    /// </summary>
    public long TotalWithdrawals { get; set; }

    /// <summary>
    /// Total distributions for the plan year in cents.
    /// </summary>
    public long TotalDistributions { get; set; }

    /// <summary>
    /// Total dividends/earnings for the plan year in cents.
    /// </summary>
    public long TotalDividends { get; set; }

    /// <summary>
    /// Total forfeitures for the plan year in cents.
    /// </summary>
    public long TotalForfeitures { get; set; }

    /// <summary>
    /// Ending balance at the end of the plan year in cents.
    /// </summary>
    public long EndingBalance { get; set; }

    /// <summary>
    /// Cumulative contribution amount from start date through end of this year in cents.
    /// </summary>
    public long CumulativeContributions { get; set; }

    /// <summary>
    /// Cumulative withdrawals from start date through end of this year in cents.
    /// </summary>
    public long CumulativeWithdrawals { get; set; }

    /// <summary>
    /// Cumulative distributions from start date through end of this year in cents.
    /// </summary>
    public long CumulativeDistributions { get; set; }
}
