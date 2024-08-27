namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncService
{
    Task SynchronizeEmployees(CancellationToken cancellationToken);
}
