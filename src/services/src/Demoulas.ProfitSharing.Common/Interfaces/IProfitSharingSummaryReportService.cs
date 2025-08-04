using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IProfitSharingSummaryReportService
{
    Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(
        BadgeNumberRequest req, CancellationToken cancellationToken = default);

    Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(
        YearEndProfitSharingReportRequest req,
        CancellationToken cancellationToken = default);

    Task<YearEndProfitSharingReportTotals> GetYearEndProfitSharingTotalsAsync(
        BadgeNumberRequest req, 
        CancellationToken cancellationToken = default);
}
