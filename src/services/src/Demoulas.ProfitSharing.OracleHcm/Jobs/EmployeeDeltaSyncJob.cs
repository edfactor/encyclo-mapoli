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
    private readonly ISet<long>? _debugOracleHcmIdSet;

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService,
        ISet<long>? debugOracleHcmIdSet) : this(employeeSyncService)
    {
        _debugOracleHcmIdSet = debugOracleHcmIdSet;
    }

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService)
    {
        _employeeSyncService = employeeSyncService;
    }


    public Task Execute(IJobExecutionContext context)
    {
        if (_debugOracleHcmIdSet?.Any() ?? false)
        {
            return _employeeSyncService.TrySyncEmployeeFromOracleHcm(requestedBy: Constants.SystemAccountName, _debugOracleHcmIdSet, context.CancellationToken);
        }
        return _employeeSyncService.ExecuteDeltaSyncAsync(requestedBy: Constants.SystemAccountName, context.CancellationToken);
    }
}
