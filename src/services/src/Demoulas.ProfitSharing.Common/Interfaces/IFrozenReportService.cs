using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IFrozenReportService
{
    Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetForfeituresAndPointsForYear(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<DistributionsByAge> GetDistributionsByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<ContributionsByAge> GetContributionsByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<ForfeituresByAge> GetForfeituresByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<BalanceByAge> GetBalanceByAgeYear(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
}
