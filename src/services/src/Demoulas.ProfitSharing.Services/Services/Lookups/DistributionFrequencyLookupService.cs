using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Lookups;

public sealed class DistributionFrequencyLookupService(IProfitSharingDataContextFactory factory) : IDistributionFrequencyLookupService
{
    public Task<ListResponseDto<DistributionFrequencyResponse>> GetDistributionFrequenciesAsync(CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var items = await ctx.DistributionFrequencies
                .OrderBy(x => x.Name)
                .Select(x => new DistributionFrequencyResponse { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
#pragma warning restore DSMPS001

            return ListResponseDto<DistributionFrequencyResponse>.From(items);
        }, cancellationToken);
    }
}
