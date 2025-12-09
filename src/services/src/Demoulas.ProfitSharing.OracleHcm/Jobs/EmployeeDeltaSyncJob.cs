using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes employee data with the Oracle HCM system.
/// </summary>
internal sealed class EmployeeDeltaSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly IProcessWatchdog _watchdog;
    private readonly ISet<long>? _debugOracleHcmIdSet;

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService,
        IProcessWatchdog watchdog,
        ISet<long>? debugOracleHcmIdSet) : this(employeeSyncService, watchdog)
    {
        _debugOracleHcmIdSet = debugOracleHcmIdSet;
    }

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService, IProcessWatchdog watchdog)
    {
        _employeeSyncService = employeeSyncService;
        _watchdog = watchdog;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (_debugOracleHcmIdSet?.Any() ?? false)
            {
                await _employeeSyncService.TrySyncEmployeeFromOracleHcm(requestedBy: Constants.SystemAccountName, _debugOracleHcmIdSet, context.CancellationToken).ConfigureAwait(false);
            }
            else
            {
                await _employeeSyncService.ExecuteDeltaSyncAsync(requestedBy: Constants.SystemAccountName, context.CancellationToken).ConfigureAwait(false);
            }

            _watchdog.RecordSuccessfulCycle();
            _watchdog.RecordHeartbeat();
        }
        catch (Exception ex)
        {
            _watchdog.RecordError($"EmployeeDeltaSyncJob failed: {ex.Message}");
            throw;
        }
    }
}
