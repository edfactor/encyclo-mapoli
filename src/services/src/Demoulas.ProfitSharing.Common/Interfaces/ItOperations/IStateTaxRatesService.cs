using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

namespace Demoulas.ProfitSharing.Common.Interfaces.ItOperations;

public interface IStateTaxRatesService
{
    Task<Result<IReadOnlyList<StateTaxRateDto>>> GetStateTaxRatesAsync(CancellationToken cancellationToken);

    Task<Result<StateTaxRateDto>> UpdateStateTaxRateAsync(UpdateStateTaxRateRequest request, CancellationToken cancellationToken);
}
