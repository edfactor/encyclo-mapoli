using Demoulas.Common.Caching;
using Demoulas.Common.Contracts.Caching;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalEntities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public sealed class StoreHostedService : BaseCacheHostedService<StoreInfoCache>
{
    private readonly IStoreService _storeService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    protected override string BaseKeyName => "STR";

    protected override ushort RefreshSeconds { get; set; } = 7200; // 2-Hourly refresh

    public StoreHostedService(IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IStoreService storeService,
        IProfitSharingDataContextFactory dataContextFactory) : base(hostEnvironment: hostEnvironment, distributedCache: distributedCache)
    {
        _storeService = storeService;
        _dataContextFactory = dataContextFactory;
    }


    public override Task<IEnumerable<StoreInfoCache>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellationToken: cancellation);
    }

    public override Task<IEnumerable<StoreInfoCache>> GetInitialDataToCacheAsync(CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellationToken: cancellation);
    }

    private async Task<IEnumerable<StoreInfoCache>> GetAllPayClassifications(CancellationToken cancellationToken)
    {
        return await _dataContextFactory.UseStoreInfoContext(async sc =>
        {
            var currentStores = await _storeService.GetCurrentStoresDictionaryAsync(sc, cancellationToken);
            var closedStores = await _storeService.GetUnopenedStoresAsync(sc, cancellationToken);

            var currentStoresIds = currentStores?.Select(c => c.Value.StoreId ?? 0) ?? new List<int>(0);
            closedStores ??= [];

            return currentStoresIds.Union(closedStores).ToHashSet().Select(s => new StoreInfoCache { StoreId = (short)s });
        });
    }
}
