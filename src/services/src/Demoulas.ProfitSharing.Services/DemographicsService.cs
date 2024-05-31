using System.Collections.Frozen;
using Demoulas.Common.Data.Contexts.DTOs.Request;
using Demoulas.Common.Data.Contexts.DTOs.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory, DemographicMapper mapper)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
    }

    public Task<PaginatedResponseDto<DemographicsResponseDto>> GetAllDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(async c =>
        {
            return await c.Demographics.Select(d => _mapper.Map(d) ).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
        });

    }

    public async Task<ISet<DemographicsResponseDto>> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        var entities = await _dataContextFactory.UseWritableContext(async context =>
        {
            var entities = _mapper.Map(demographics).ToList();
            context.Demographics.AddRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            return entities;
        }, cancellationToken);

        return _mapper.Map(entities).ToFrozenSet();
    }
}
