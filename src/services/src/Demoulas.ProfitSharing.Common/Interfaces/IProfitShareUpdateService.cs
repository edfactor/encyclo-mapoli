using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IProfitShareUpdateService
{
    public Task<ProfitShareUpdateResponse> ProfitShareUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);
}
