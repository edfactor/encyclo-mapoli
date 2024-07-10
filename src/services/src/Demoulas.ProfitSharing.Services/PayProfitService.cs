using System.Collections.Frozen;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
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
    private readonly DemographicMapper _mapper;
    private readonly ILogger _logger;

    public PayProfitService(IProfitSharingDataContextFactory dataContextFactory, DemographicMapper mapper, ILogger logger)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResponseDto<DemographicsResponseDto>?> GetAllProfits(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request all Pay Profit records"))
        {
            return await _dataContextFactory.UseReadOnlyContext(async c =>
            {
                return await c.Demographics.Select(d => _mapper.Map(d)).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
            });
        }
    }

    public async Task<ISet<DemographicsResponseDto>?> AddProfit(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        /*
         *
         * TODO: This needs to be an upsert. People who have retired will stay in Profit sharing, but would be removed from 
         *
         *
         */


        List<PayProfit> entities = await _dataContextFactory.UseWritableContext(async context =>
        {
            List<PayProfit> entities = _mapper.Map(demographics).ToList();
            context.PayProfits.AddRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            return entities;
        }, cancellationToken);

        return _mapper.Map(entities).ToFrozenSet();
    }
}
