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
    /// <param name="request">Request containing date range and member identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account history report response with activity by plan year</returns>
    Task<ReportResponseBase<AccountHistoryReportResponse>> GetAccountHistoryReportAsync(
        int memberId,
        StartAndEndDateRequest request,
        CancellationToken cancellationToken);
}
