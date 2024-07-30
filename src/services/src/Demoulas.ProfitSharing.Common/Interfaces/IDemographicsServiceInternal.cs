using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsServiceInternal : IDemographicsService
{
    Task AddDemographicsStream(IAsyncEnumerable<DemographicsRequestDto> employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default);
}
