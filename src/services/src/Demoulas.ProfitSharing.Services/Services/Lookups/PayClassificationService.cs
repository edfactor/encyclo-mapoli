using System.Collections.Frozen;
using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Caching.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Services.Services.Lookups;

public sealed class PayClassificationService : IPayClassificationService
{
    private readonly IBaseCacheService<LookupTableCache<string>> _accountCache;

    public PayClassificationService([FromKeyedServices(nameof(PayClassificationHostedService))] IBaseCacheService<LookupTableCache<string>> accountCache)
    {
        _accountCache = accountCache;
    }

    public async Task<ISet<PayClassificationResponseDto>> GetAllPayClassificationsAsync(CancellationToken cancellationToken = default)
    {
        ISet<LookupTableCache<string>> arcobjects = await _accountCache.GetAllAsync(cancellationToken);
        return arcobjects.Select(o => new PayClassificationResponseDto { Id = o.Id, Name = o.Name }).ToFrozenSet();
    }
}
