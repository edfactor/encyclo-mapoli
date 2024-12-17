namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncService
{
    Task SynchronizeEmployeesAsync(string requestedBy = "System", CancellationToken cancellationToken = default);
}
