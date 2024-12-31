using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsServiceInternal
{
    Task AddDemographicsStreamAsync(IAsyncEnumerable<DemographicsRequest> employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default);

    void ProcessDemographics(DeltaContext record);
}
