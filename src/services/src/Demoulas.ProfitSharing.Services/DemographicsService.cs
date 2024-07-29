using System.Collections.Frozen;
using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory, 
        DemographicMapper mapper,
        ILogger<DemographicsService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task AddDemographicsStream(IAsyncEnumerable<DemographicsRequestDto> employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(AddDemographicsStream), ActivityKind.Internal);
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
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(AddDemographics), ActivityKind.Internal);
        try
        {


            List<Demographic> entities = await _dataContextFactory.UseWritableContext(async context =>
            {
                List<Demographic> entities = _mapper.Map(demographics).ToList();
                context.Demographics.AddRange(entities);
                await context.SaveChangesAsync(cancellationToken);
                return entities;
            }, cancellationToken);

            return _mapper.Map(entities).ToFrozenSet();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to add to the {Demographics} table: {message}.", nameof(Demographic), e.Message);
            throw;
        }
    }
}
