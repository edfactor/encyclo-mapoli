using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Paginated response for Account History Report that includes cumulative totals
/// calculated across all results (not just the current page).
/// </summary>
public record AccountHistoryReportPaginatedResponse : PaginatedResponseDto<AccountHistoryReportResponse>
{
    /// <summary>
    /// Cumulative totals for the member's account across the date range (all years).
    /// These are calculated from all filtered results before pagination.
    /// </summary>
    public AccountHistoryReportTotals? CumulativeTotals { get; set; }
}
