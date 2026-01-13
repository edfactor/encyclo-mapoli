using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

public interface IFakeSsnService
{
    Task<int> GenerateFakeSsnAsync(CancellationToken cancellationToken);
    Task<int[]> GenerateFakeSsnBatchAsync(int batchSize, CancellationToken cancellationToken);
    Task TrackSsnChangeAsync<THistory>(int oldSsn, int newSsn, CancellationToken cancellationToken) where THistory : SsnChangeHistory, new();
}
