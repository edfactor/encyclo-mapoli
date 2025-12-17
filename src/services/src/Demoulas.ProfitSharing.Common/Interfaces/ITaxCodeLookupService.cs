using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ITaxCodeLookupService
{
    Task<ListResponseDto<TaxCodeResponse>> GetTaxCodesAsync(CancellationToken cancellationToken = default);
}
