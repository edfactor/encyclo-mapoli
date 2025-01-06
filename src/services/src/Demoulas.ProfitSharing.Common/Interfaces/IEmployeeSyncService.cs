namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncService
{
    Task ExecuteFullSyncAsync(string requestedBy = "System", CancellationToken cancellationToken = default);
    Task ExecuteDeltaSyncAsync(string requestedBy = "System", CancellationToken cancellationToken = default);
}
