using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ICleanupReportService
{
    Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingCommaAsync(SortedPaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfitAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<DistributionsAndForfeitureTotalsResponse> GetDistributionsAndForfeitureAsync(DistributionsAndForfeituresRequest req, CancellationToken cancellationToken = default);
}
