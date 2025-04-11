using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ICleanupReportService
{
    Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(SortedPaginationRequestDto req, CancellationToken ct);

    Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponseAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingCommaAsync(SortedPaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(YearEndProfitSharingReportRequest req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfitAsync(SortedPaginationRequestDto req,CancellationToken cancellationToken = default);
    Task<DistributionsAndForfeitureTotalsResponse> GetDistributionsAndForfeitureAsync(DistributionsAndForfeituresRequest req, CancellationToken cancellationToken = default);
    Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(FrozenProfitYearRequest req, CancellationToken cancellationToken = default);
}
