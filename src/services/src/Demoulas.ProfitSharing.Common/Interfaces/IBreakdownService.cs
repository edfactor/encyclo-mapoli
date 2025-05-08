using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IBreakdownService
{
    Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);
    Task<BreakdownByStoreTotals> GetTotalsByStore(BreakdownByStoreRequest request, CancellationToken cancellationToken);
}
