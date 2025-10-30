using Demoulas.Common.Contracts.Contracts.Request;
using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request DTO for account history report queries.
/// Accepts request body with badge number, date range, and pagination parameters.
/// </summary>
public record AccountHistoryReportRequest : SortedPaginationRequestDto
{
    /// <summary>
    /// The badge number of the employee/member to generate the report for.
    /// </summary>
    public int BadgeNumber { get; set; }

    /// <summary>
    /// The beginning date for the report date range (optional, defaults to 1/1/2007 or earliest record).
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// The ending date for the report date range (optional, defaults to today).
    /// </summary>
    public DateOnly? EndDate { get; set; }
}
