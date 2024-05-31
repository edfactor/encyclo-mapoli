using Demoulas.Common.Caching;
using Demoulas.Common.Contracts.Caching;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public sealed class PayClassificationHostedService : BaseCacheHostedService<PayClassificationResponseCache>
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    protected override string BaseKeyName => "ACS";

    protected override ushort RefreshSeconds { get; set; } = 3600; // Hourly refresh

    public PayClassificationHostedService(IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IProfitSharingDataContextFactory contextFactory) : base(hostEnvironment, distributedCache)
    {
        _contextFactory = contextFactory;
    }


    public override Task<IEnumerable<PayClassificationResponseCache>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellation);
    }

    public override Task<IEnumerable<PayClassificationResponseCache>> GetInitialDataToCacheAsync(CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellation);
    }

    private async Task<IEnumerable<PayClassificationResponseCache>> GetAllPayClassifications(CancellationToken cancellationToken)
    {
        return await _contextFactory.UseReadOnlyContext(context =>
        {
            return context.PayClassifications
                .Select(c => new PayClassificationResponseCache
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        });
    }
}
