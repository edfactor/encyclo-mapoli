using Demoulas.ProfitSharing.Common.Interfaces;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes employee data with the Oracle HCM system.
/// </summary>
internal sealed class EmployeeFullSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;

    public EmployeeFullSyncJob(IEmployeeSyncService employeeSyncService)
    {
        _employeeSyncService = employeeSyncService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _employeeSyncService.SynchronizeEmployeesAsync(requestedBy: "System", context.CancellationToken);
    }
}
