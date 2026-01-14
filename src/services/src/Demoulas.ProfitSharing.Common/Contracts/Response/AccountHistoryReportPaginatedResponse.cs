using Demoulas.Common.Contracts.Contracts.Response;
namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Paginated response for Account History Report that includes cumulative totals
/// calculated across all results (not just the current page).
/// </summary>
public record AccountHistoryReportPaginatedResponse : ReportResponseBase<AccountHistoryReportResponse>
{
    public AccountHistoryReportTotals? CumulativeTotals { get; set; }

    public static AccountHistoryReportPaginatedResponse ResponseExample() => new()
    {
        ReportName = "Account History Report",
        ReportDate = DateTimeOffset.UtcNow,
        StartDate = new DateOnly(2024, 1, 1),
        EndDate = new DateOnly(2024, 12, 31),
        CumulativeTotals = AccountHistoryReportTotals.ResponseExample(),
        Response = new PaginatedResponseDto<AccountHistoryReportResponse>
        {
            Results = new List<AccountHistoryReportResponse> { AccountHistoryReportResponse.ResponseExample() }
        }
    };
}
