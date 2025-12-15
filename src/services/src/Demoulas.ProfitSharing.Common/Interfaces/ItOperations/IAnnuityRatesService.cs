using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

namespace Demoulas.ProfitSharing.Common.Interfaces.ItOperations;

public interface IAnnuityRatesService
{
    Task<Result<IReadOnlyList<AnnuityRateDto>>> GetAnnuityRatesAsync(CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<AnnuityRateDto>>> GetAnnuityRatesByYearAsync(short year, CancellationToken cancellationToken);

    Task<Result<AnnuityRateDto>> UpdateAnnuityRateAsync(UpdateAnnuityRateRequest request, CancellationToken cancellationToken);
}
