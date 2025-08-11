using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IBreakdownService
{
    Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);
    Task<ReportResponseBase<MemberYearSummaryDto>> GetMembersWithBalanceByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);
    Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithBalanceByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersWithBalanceByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken);

    Task<BreakdownByStoreTotals> GetTotalsByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);

    public Task<GrandTotalsByStoreResponseDto> GetGrandTotals(YearRequest request, CancellationToken cancellationToken);
}
