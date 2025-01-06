using Demoulas.ProfitSharing.Common.Interfaces;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes employee data with the Oracle HCM system.
/// </summary>
internal sealed class EmployeeDeltaSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService)
    {
        _employeeSyncService = employeeSyncService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _employeeSyncService.ExecuteDeltaSyncAsync(requestedBy: "System", context.CancellationToken);
    }
}
