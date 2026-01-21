using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

namespace Demoulas.ProfitSharing.Common.Interfaces.Administration;

public interface ITaxCodeService
{
    Task<Result<IReadOnlyList<TaxCodeAdminDto>>> GetTaxCodesAsync(CancellationToken cancellationToken);

    Task<Result<TaxCodeAdminDto>> CreateTaxCodeAsync(CreateTaxCodeRequest request, CancellationToken cancellationToken);

    Task<Result<TaxCodeAdminDto>> UpdateTaxCodeAsync(UpdateTaxCodeRequest request, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteTaxCodeAsync(char id, CancellationToken cancellationToken);
}
