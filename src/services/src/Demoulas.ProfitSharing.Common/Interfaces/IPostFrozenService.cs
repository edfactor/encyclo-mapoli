using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IPostFrozenService
{
    Task<ProfitSharingUnder21ReportResponse> ProfitSharingUnder21Report(ProfitYearRequest request, CancellationToken cancellationToken);
}
