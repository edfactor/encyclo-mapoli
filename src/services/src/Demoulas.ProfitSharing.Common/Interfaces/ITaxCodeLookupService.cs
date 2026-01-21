using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ITaxCodeLookupService
{
    Task<ListResponseDto<TaxCodeResponse>> GetTaxCodesAsync(TaxCodeLookupRequest request, CancellationToken cancellationToken = default);
}
