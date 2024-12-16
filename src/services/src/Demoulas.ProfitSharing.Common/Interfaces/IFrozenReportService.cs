using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IFrozenReportService
{
    Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetForfeituresAndPointsForYearAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<DistributionsByAge> GetDistributionsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<ContributionsByAge> GetContributionsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<ForfeituresByAge> GetForfeituresByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<BalanceByAge> GetBalanceByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<VestedAmountsByAge> GetVestedAmountsByAgeYearAsync(ProfitYearRequest req, CancellationToken cancellationToken = default);
}
