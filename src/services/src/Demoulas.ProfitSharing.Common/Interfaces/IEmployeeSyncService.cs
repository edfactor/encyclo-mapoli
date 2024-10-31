namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncService
{
    Task SynchronizeEmployees(string requestedBy = "System", CancellationToken cancellationToken = default);
}
