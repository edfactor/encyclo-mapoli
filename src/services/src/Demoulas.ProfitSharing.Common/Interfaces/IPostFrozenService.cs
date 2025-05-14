using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IPostFrozenService
{
    Task<ProfitSharingUnder21ReportResponse> ProfitSharingUnder21Report(ProfitYearRequest request, CancellationToken cancellationToken);
    Task<ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>> ProfitSharingUnder21BreakdownByStore(ProfitYearRequest request, CancellationToken cancellation);
    Task<ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>> ProfitSharingUnder21InactiveNoBalance(FrozenProfitYearRequest request, CancellationToken cancellationToken);
    Task<ProfitSharingUnder21TotalsResponse> GetUnder21Totals(FrozenProfitYearRequest request, CancellationToken cancellationToken);
    Task<PaginatedResponseDto<NewProfitSharingLabelResponse>> GetNewProfitSharingLabels(ProfitYearRequest request, CancellationToken cancellationToken);
    Task<List<string>> GetNewProfitSharingLabelsForMailMerge(ProfitYearRequest request, CancellationToken ct);
    Task<PaginatedResponseDto<ProfitSharingLabelResponse>> GetProfitSharingLabels(FrozenProfitYearRequest request, CancellationToken ct);
    Task<List<string>> GetProfitSharingLabelsExport(FrozenProfitYearRequest request, CancellationToken ct);
}
