using System.Collections.Frozen;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public class PayProfitService : IPayProfitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly PayProfitMapper _mapper;

    public PayProfitService(IProfitSharingDataContextFactory dataContextFactory, PayProfitMapper mapper)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
    }

    public async Task<ISet<PayProfitResponseDto>?> AddProfit(IEnumerable<PayProfitRequestDto> profitRequest, CancellationToken cancellationToken)
    {
        List<PayProfitLegacy> entities = await _dataContextFactory.UseWritableContext(async context =>
        {
            List<PayProfitLegacy> entities = _mapper.Map(profitRequest).ToList();
            context.PayProfits.AddRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            return entities;
        }, cancellationToken);

        return _mapper.Map(entities).ToFrozenSet();
    }
}
