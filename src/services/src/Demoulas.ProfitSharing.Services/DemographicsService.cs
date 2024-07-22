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

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService : IDemographicsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory, DemographicMapper mapper)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
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
