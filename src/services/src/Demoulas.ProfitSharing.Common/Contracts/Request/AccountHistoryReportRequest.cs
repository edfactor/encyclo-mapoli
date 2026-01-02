using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request DTO for account history report queries.
/// Accepts request body with badge number, date range, and pagination parameters.
/// </summary>
public record AccountHistoryReportRequest : SortedPaginationRequestDto, IBadgeNumberRequest
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

    public static AccountHistoryReportRequest RequestExample()
    {
        return new AccountHistoryReportRequest
        {
            BadgeNumber = 123456,
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            Skip = 0,
            Take = 50,
            SortBy = "Date",
            IsSortDescending = true
        };
    }
}
