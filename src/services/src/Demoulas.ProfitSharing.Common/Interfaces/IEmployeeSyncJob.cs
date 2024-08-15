namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeSyncJob
{
    Task SynchronizeEmployees(CancellationToken cancellationToken);
}
