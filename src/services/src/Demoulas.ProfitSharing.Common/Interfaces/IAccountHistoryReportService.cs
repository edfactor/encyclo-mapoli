using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for generating account history report with member account activity by plan year.
/// </summary>
public interface IAccountHistoryReportService
{
    /// <summary>
    /// Generates an account history report for a member with account activity condensed by profit year.
    /// </summary>
    /// <param name="memberId">The member's unique identifier (OracleHcmId or badge number)</param>
    /// <param name="request">Request containing date range, badge number, and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account history report response with activity by plan year and cumulative totals</returns>
    Task<AccountHistoryReportPaginatedResponse> GetAccountHistoryReportAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Generates a PDF report for the account history data.
    /// </summary>
    /// <param name="memberId">The member badge number</param>
    /// <param name="request">The account history report request with date range and sorting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Memory stream containing the PDF report</returns>
    Task<MemoryStream> GeneratePdfAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken);
}
