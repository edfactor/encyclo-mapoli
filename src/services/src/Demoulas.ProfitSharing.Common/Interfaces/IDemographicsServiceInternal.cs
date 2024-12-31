using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.OracleHcm.Atom;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsServiceInternal
{
    Task AddDemographicsStreamAsync(IAsyncEnumerable<DemographicsRequest> employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default);

    void ProcessDemographics(Context record);
}
