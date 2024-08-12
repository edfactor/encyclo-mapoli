namespace Demoulas.ProfitSharing.Services.Jobs;

public interface IEmployeeSyncJob
{
    Task SynchronizeEmployee(int badgeNumber, CancellationToken cancellationToken);
    Task SynchronizeEmployees(CancellationToken cancellationToken);
}
