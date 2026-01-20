using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

public sealed class TaxCodeLookupService(IProfitSharingDataContextFactory factory) : ITaxCodeLookupService
{
    public Task<ListResponseDto<TaxCodeResponse>> GetTaxCodesAsync(TaxCodeLookupRequest request, CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var query = ctx.TaxCodes
                .TagWith("Lookup-TaxCodes")
                .AsQueryable();

            var includeDistribution = request.AvailableForDistribution == true;
            var includeForfeiture = request.AvailableForForfeiture == true;

            if (includeDistribution || includeForfeiture)
            {
                query = query.Where(x => (includeDistribution && x.IsAvailableForDistribution) ||
                                         (includeForfeiture && x.IsAvailableForForfeiture));
            }

            var items = await query
                .OrderBy(x => x.Name)
                .Select(x => new TaxCodeResponse { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
#pragma warning restore DSMPS001

            return ListResponseDto<TaxCodeResponse>.From(items);
        }, cancellationToken);
    }
}
