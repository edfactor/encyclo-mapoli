using Demoulas.Common.Caching;
using Demoulas.Common.Contracts.Caching;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Caching.HostedServices;

public sealed class DepartmentHostedService : BaseCacheHostedService<LookupTableCache<byte>>
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    protected override string BaseKeyName => "DEP";

    protected override ushort RefreshSeconds { get; set; } = 7200; // Every two hours refresh

    public DepartmentHostedService(IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IProfitSharingDataContextFactory contextFactory) : base(hostEnvironment: hostEnvironment, distributedCache: distributedCache)
    {
        _contextFactory = contextFactory;
    }


    public override Task<IEnumerable<LookupTableCache<byte>>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default)
    {
        return GetAllDepartments(cancellationToken: cancellation);
    }

    public override Task<IEnumerable<LookupTableCache<byte>>> GetInitialDataToCacheAsync(CancellationToken cancellation = default)
    {
        return GetAllDepartments(cancellationToken: cancellation);
    }

    private async Task<IEnumerable<LookupTableCache<byte>>> GetAllDepartments(CancellationToken cancellationToken)
    {
        return await _contextFactory.UseReadOnlyContext(func: async context =>
        {
            bool canConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return [];
            }

            return await context.Departments
                .Select(selector: c => new LookupTableCache<byte> { Id = c.Id, Name = c.Name })
                .ToListAsync(cancellationToken: cancellationToken);
        }, cancellationToken);
    }
}
