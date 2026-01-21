using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Lookup;

public sealed class DistributionStatusLookupService(IProfitSharingDataContextFactory factory) : IDistributionStatusLookupService
{
    public Task<ListResponseDto<DistributionStatusResponse>> GetDistributionStatusesAsync(CancellationToken cancellationToken = default)
    {
        return factory.UseReadOnlyContext(async ctx =>
        {
#pragma warning disable DSMPS001
            var items = await ctx.DistributionStatuses
                .OrderBy(x => x.Name)
                .Select(x => new DistributionStatusResponse { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
#pragma warning restore DSMPS001

            return ListResponseDto<DistributionStatusResponse>.From(items);
        }, cancellationToken);
    }
}
