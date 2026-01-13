using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IFrozenReportService
{
    Task<DistributionsByAge> GetDistributionsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<ContributionsByAge> GetContributionsByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<ForfeituresByAge> GetForfeituresByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<BalanceByAge> GetBalanceByAgeYearAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<BalanceByYears> GetBalanceByYearsAsync(FrozenReportsByAgeRequest req, CancellationToken cancellationToken = default);
    Task<VestedAmountsByAge> GetVestedAmountsByAgeYearAsync(ProfitYearAndAsOfDateRequest req, CancellationToken cancellationToken = default);
    Task<GrossWagesReportResponse> GetGrossWagesReport(GrossWagesReportRequest req, CancellationToken cancellationToken = default);
    Task<UpdateSummaryReportResponse> GetUpdateSummaryReport(ProfitYearRequest req, CancellationToken cancellationToken = default);
    Task<ProfitControlSheetResponse> GetProfitControlSheet(ProfitYearRequest request, CancellationToken cancellationToken = default);
}
