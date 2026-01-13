using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes employee data with the Oracle HCM system.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class EmployeeDeltaSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly IProcessWatchdog _watchdog;
    private readonly ILogger<EmployeeDeltaSyncJob> _logger;
    private readonly ISet<long>? _debugOracleHcmIdSet;

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService,
        IProcessWatchdog watchdog,
        ILogger<EmployeeDeltaSyncJob> logger,
        ISet<long>? debugOracleHcmIdSet) : this(employeeSyncService, watchdog, logger)
    {
        _debugOracleHcmIdSet = debugOracleHcmIdSet;
    }

    public EmployeeDeltaSyncJob(IEmployeeSyncService employeeSyncService, IProcessWatchdog watchdog, ILogger<EmployeeDeltaSyncJob> logger)
    {
        _employeeSyncService = employeeSyncService;
        _watchdog = watchdog;
        _logger = logger;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        _watchdog.RecordHeartbeat();

        try
        {
            ResiliencePipeline pipeline = OracleHcmRetryPolicy.Create(nameof(EmployeeDeltaSyncJob), _logger, _watchdog);

            ResilienceContext resilienceContext = ResilienceContextPool.Shared.Get(context.CancellationToken);
            try
            {
                await pipeline.ExecuteAsync(async (ResilienceContext ctx) =>
                {
                    CancellationToken ct = ctx.CancellationToken;
                    if (_debugOracleHcmIdSet?.Any() ?? false)
                    {
                        await _employeeSyncService.TrySyncEmployeeFromOracleHcm(requestedBy: Constants.SystemAccountName, _debugOracleHcmIdSet, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        await _employeeSyncService.ExecuteDeltaSyncAsync(requestedBy: Constants.SystemAccountName, ct).ConfigureAwait(false);
                    }
                }, resilienceContext).ConfigureAwait(false);
            }
            finally
            {
                ResilienceContextPool.Shared.Return(resilienceContext);
            }

            _watchdog.RecordSuccessfulCycle();
        }
        catch (Exception ex)
        {
            _watchdog.RecordError($"EmployeeDeltaSyncJob failed. IsTransient={OracleHcmTransientFaultDetector.IsTransient(ex)} ExceptionType={ex.GetType().Name}");
            _logger.LogError(ex, "EmployeeDeltaSyncJob failed");
            throw new JobExecutionException("EmployeeDeltaSyncJob failed", ex, refireImmediately: false);
        }
        finally
        {
            _watchdog.RecordHeartbeat();
        }
    }
}
