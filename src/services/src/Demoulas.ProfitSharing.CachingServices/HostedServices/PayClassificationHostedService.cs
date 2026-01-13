using Demoulas.Common.Caching;
using Demoulas.Common.Contracts.Caching;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Caching.HostedServices;

public sealed class PayClassificationHostedService : BaseCacheHostedService<LookupTableCache<string>>
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    protected override string BaseKeyName => "ACS";

    protected override ushort RefreshSeconds { get; set; } = 7200; // Every two hours refresh

    public PayClassificationHostedService(IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IProfitSharingDataContextFactory contextFactory) : base(hostEnvironment: hostEnvironment, distributedCache: distributedCache)
    {
        _contextFactory = contextFactory;
    }


    public override Task<IEnumerable<LookupTableCache<string>>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellationToken: cancellation);
    }

    public override Task<IEnumerable<LookupTableCache<string>>> GetInitialDataToCacheAsync(CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellationToken: cancellation);
    }

    private async Task<IEnumerable<LookupTableCache<string>>> GetAllPayClassifications(CancellationToken cancellationToken)
    {
        return await _contextFactory.UseReadOnlyContext(func: async context =>
        {
            bool canConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return [];
            }

            return await context.PayClassifications
                .Select(selector: c => new LookupTableCache<string> { Id = c.Id, Name = c.Name })
                .ToListAsync(cancellationToken: cancellationToken);
        }, cancellationToken);
    }
}
