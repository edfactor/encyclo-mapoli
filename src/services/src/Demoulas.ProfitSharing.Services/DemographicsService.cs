using System.Collections.Frozen;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory, DemographicMapper mapper)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
    }

    public async Task AddDemographicsStream(IAsyncEnumerable<DemographicsRequestDto> employees, byte batchSize = byte.MaxValue, CancellationToken cancellationToken = default)
    {
        var batch = new List<DemographicsRequestDto>();

        await foreach (var employee in employees.WithCancellation(cancellationToken))
        {
            batch.Add(employee);

            if (batch.Count >= batchSize)
            {
                _ = await AddDemographics(batch, cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            _ = await AddDemographics(batch, cancellationToken);
        }
    }

    public async Task<ISet<DemographicsResponseDto>?> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        /*
         *
         * TODO: This needs to be an upsert. People who have retired will stay in Profit sharing, but would be removed from 
         *
         *
         */


        List<Demographic> entities = await _dataContextFactory.UseWritableContext(async context =>
        {
            List<Demographic> entities = _mapper.Map(demographics).ToList();
            context.Demographics.AddRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            return entities;
        }, cancellationToken);

        return _mapper.Map(entities).ToFrozenSet();
    }
}
