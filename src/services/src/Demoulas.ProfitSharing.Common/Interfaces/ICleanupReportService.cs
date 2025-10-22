using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ICleanupReportService
{
    Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfitAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<Result<DistributionsAndForfeitureTotalsResponse>> GetDistributionsAndForfeitureAsync(DistributionsAndForfeituresRequest req, CancellationToken cancellationToken = default);
}
