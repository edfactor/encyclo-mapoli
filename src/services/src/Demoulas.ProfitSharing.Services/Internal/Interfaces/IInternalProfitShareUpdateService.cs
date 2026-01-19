using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;

namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

public interface IInternalProfitShareUpdateService : IProfitShareUpdateService
{
    // Internal access by other services (includes ssn)
    internal Task<ProfitShareUpdateResult> ProfitShareUpdateInternalAsync(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken);

}
