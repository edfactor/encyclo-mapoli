using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsServiceInternal
{
    Task AddDemographicsStream(IAsyncEnumerable<DemographicsRequest> employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default);

    Task<DateTime?> GetLastOracleHcmSyncDate(CancellationToken cancellationToken = default);
}
