using System.Collections.Frozen;
using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.InternalEntities;

namespace Demoulas.ProfitSharing.Services;

public sealed class PayClassificationService : IPayClassificationService
{
    private readonly IBaseCacheService<PayClassificationResponseCache> _accountCache;

    public PayClassificationService(IBaseCacheService<PayClassificationResponseCache> accountCache)
    {
        _accountCache = accountCache;
    }

    public async Task<ISet<PayClassificationResponseDto>> GetAllPayClassifications(CancellationToken cancellationToken = default)
    {
        ISet<PayClassificationResponseCache> arcobjects =await  _accountCache.GetAllAsync(cancellationToken);
        return arcobjects.Select(o=> new PayClassificationResponseDto
        {
            Id = o.Id,
            Name = o.Name
        }).ToFrozenSet();
    }
}
