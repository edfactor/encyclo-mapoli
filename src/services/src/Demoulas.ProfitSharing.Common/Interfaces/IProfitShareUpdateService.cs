using Demoulas.ProfitSharing.Common.Contracts.InternalDto;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IProfitShareUpdateService
{
    // External access by Web app (no ssn)
    public Task<ProfitShareUpdateResponse> ProfitShareUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);

    // Internal access by other services (includes ssn)
    public Task<ProfitShareUpdateResult> ProfitShareUpdateInternal(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);

}
