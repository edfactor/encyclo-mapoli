using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IFrozenReportService
{
    public Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetForfeituresAndPointsForYear(ProfitYearRequest req, CancellationToken cancellationToken = default);
    public Task<ReportResponseBase<ProfitSharingDistributionsByAge>> GetDistributionsByAgeYear(ProfitYearRequest req, CancellationToken cancellationToken = default);
}
