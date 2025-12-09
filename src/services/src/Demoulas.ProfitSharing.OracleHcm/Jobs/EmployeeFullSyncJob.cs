using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes employee data with the Oracle HCM system.
/// </summary>
internal sealed class EmployeeFullSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly IProcessWatchdog _watchdog;

    public EmployeeFullSyncJob(IEmployeeSyncService employeeSyncService, IProcessWatchdog watchdog)
    {
        _employeeSyncService = employeeSyncService;
        _watchdog = watchdog;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _employeeSyncService.ExecuteFullSyncAsync(requestedBy: Constants.SystemAccountName, context.CancellationToken).ConfigureAwait(false);
            _watchdog.RecordSuccessfulCycle();
            _watchdog.RecordHeartbeat();
        }
        catch (Exception ex)
        {
            _watchdog.RecordError($"EmployeeFullSyncJob failed: {ex.Message}");
            throw;
        }
    }
}
