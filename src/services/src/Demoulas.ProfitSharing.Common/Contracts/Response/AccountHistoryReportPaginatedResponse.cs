using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Paginated response for Account History Report that includes cumulative totals
/// calculated across all results (not just the current page).
/// </summary>
public record AccountHistoryReportPaginatedResponse : ReportResponseBase<AccountHistoryReportResponse>
{
    public AccountHistoryReportTotals? CumulativeTotals { get; set; }
}
