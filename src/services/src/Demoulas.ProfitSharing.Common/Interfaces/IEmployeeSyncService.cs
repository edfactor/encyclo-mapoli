using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncService
{
    Task ExecuteFullSyncAsync(string requestedBy = "System", CancellationToken cancellationToken = default);
    Task ExecuteDeltaSyncAsync(string requestedBy = "System", CancellationToken cancellationToken = default);
    ValueTask QueueEmployee(string requestedBy, OracleEmployee[] employees, string jobName, CancellationToken cancellationToken);
    Task TrySyncEmployeeFromOracleHcm(string requestedBy, ISet<long> people, CancellationToken cancellationToken);
}
