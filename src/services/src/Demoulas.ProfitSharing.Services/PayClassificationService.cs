using Demoulas.Common.Caching.Interfaces;
using Demoulas.Common.Data.Contexts.DTOs.Request;
using Demoulas.Common.Data.Contexts.DTOs.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Services;

public class PayClassificationService
{
    private readonly IBaseCacheService<PayClassificationResponseDto> _accountCache;

    public PayClassificationService(IBaseCacheService<PayClassificationResponseDto> accountCache)
    {
        _accountCache = accountCache;
    }

    public Task<ISet<PayClassificationResponseDto>> GetAllPayClassifications(CancellationToken cancellationToken = default)
    {
        return _accountCache.GetAllAsync(cancellationToken);
    }
}
