using Demoulas.ProfitSharing.OracleHcm.Atom;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes employee data with the Oracle HCM system.
/// </summary>
internal sealed class EmployeeDeltaSyncJob : IJob
{
    private readonly SyncJobService _employeeSyncService;

    public EmployeeDeltaSyncJob(SyncJobService employeeSyncService)
    {
        _employeeSyncService = employeeSyncService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _employeeSyncService.ExecuteDeltaSyncAsync(context.CancellationToken);
    }
}
