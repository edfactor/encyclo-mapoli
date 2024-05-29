using Demoulas.Common.Caching;
using Demoulas.Common.Caching.Helpers;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Contracts.Contracts.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public sealed class PayClassificationHostedService : BaseCacheHostedService<PayClassificationResponseDto>
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


    public override Task<IEnumerable<PayClassificationResponseDto>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellation);
    }

    public override Task<IEnumerable<PayClassificationResponseDto>> GetInitialDataToCacheAsync(CancellationToken cancellation = default)
    {
        return GetAllPayClassifications(cancellation);
    }

    private async Task<IEnumerable<PayClassificationResponseDto>> GetAllPayClassifications(CancellationToken cancellationToken)
    {
        return await _contextFactory.UseReadOnlyContext(context =>
        {
            return context.PayClassifications
                .Select(c => new PayClassificationResponseDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        });
    }
}
