using System.Collections.Frozen;
using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.HostedServices;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Services;

public sealed class PayClassificationService : IPayClassificationService
{
    private readonly IBaseCacheService<LookupTableCache<byte>> _accountCache;

    public PayClassificationService([FromKeyedServices(nameof(PayClassificationHostedService))] IBaseCacheService<LookupTableCache<byte>> accountCache)
    {
        _accountCache = accountCache;
    }

    public async Task<ISet<PayClassificationResponseDto>> GetAllPayClassificationsAsync(CancellationToken cancellationToken = default)
    {
        ISet<LookupTableCache<byte>> arcobjects =await  _accountCache.GetAllAsync(cancellationToken);
        return arcobjects.Select(o=> new PayClassificationResponseDto
        {
            Id = o.Id,
            Name = o.Name
        }).ToFrozenSet();
    }
}
