using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IBreakdownService
{
    Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);
    Task<ReportResponseBase<MemberYearSummaryDto>> GetMembersWithBalanceActivityByStore(BreakdownByStoreRequest request, int[]? Ssns, int[] BadgeNumbers, CancellationToken cancellationToken);
    Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithVestedBalanceByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersWithVestedBalanceByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetRetiredEmployessWithBalanceActivity(
       TerminatedEmployeesWithBalanceBreakdownRequest request,
       CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithCurrentBalanceNotVestedByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithBeneficiaryByStore(
       TerminatedEmployeesWithBalanceBreakdownRequest request,
       CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithBalanceActivityByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken);

    Task<ReportResponseBase<MemberYearSummaryDto>> GetMonthlyEmployeesWithActivity(
       TerminatedEmployeesWithBalanceBreakdownRequest request,
       CancellationToken cancellationToken);

    Task<BreakdownByStoreTotals> GetTotalsByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);

    public Task<GrandTotalsByStoreResponseDto> GetGrandTotals(YearRequest request, CancellationToken cancellationToken);
}
