using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

namespace Demoulas.ProfitSharing.Common.Interfaces.ItOperations;

public interface IAnnuityRatesService
{
    Task<Result<IReadOnlyList<AnnuityRateDto>>> GetAnnuityRatesAsync(GetAnnuityRatesRequest request, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<AnnuityRateDto>>> GetAnnuityRatesByYearAsync(short year, CancellationToken cancellationToken);

    Task<Result<AnnuityRateDto>> UpdateAnnuityRateAsync(UpdateAnnuityRateRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Checks which years have complete annuity rate data (all required ages defined).
    /// </summary>
    /// <param name="request">Contains optional start/end year range. Defaults to current year and previous 5 years.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completeness status for each year in the range.</returns>
    Task<Result<MissingAnnuityYearsResponse>> GetMissingAnnuityYearsAsync(GetMissingAnnuityYearsRequest request, CancellationToken cancellationToken);
}
