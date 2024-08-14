namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncJob
{
    Task<bool> SynchronizeEmployee(int badgeNumber, CancellationToken cancellationToken);
    Task SynchronizeEmployees(CancellationToken cancellationToken);
}
