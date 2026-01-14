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
internal sealed class EmployeeFullSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly IProcessWatchdog _watchdog;
    private readonly ILogger<EmployeeFullSyncJob> _logger;

    public EmployeeFullSyncJob(IEmployeeSyncService employeeSyncService, IProcessWatchdog watchdog, ILogger<EmployeeFullSyncJob> logger)
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
            ResiliencePipeline pipeline = OracleHcmRetryPolicy.Create(nameof(EmployeeFullSyncJob), _logger, _watchdog);

            ResilienceContext resilienceContext = ResilienceContextPool.Shared.Get(context.CancellationToken);
            try
            {
                await pipeline.ExecuteAsync(
                    async (ResilienceContext ctx) =>
                    {
                        CancellationToken ct = ctx.CancellationToken;
                        await _employeeSyncService.ExecuteFullSyncAsync(requestedBy: Constants.SystemAccountName, ct).ConfigureAwait(false);
                    },
                    resilienceContext).ConfigureAwait(false);
            }
            finally
            {
                ResilienceContextPool.Shared.Return(resilienceContext);
            }

            _watchdog.RecordSuccessfulCycle();
        }
        catch (Exception ex)
        {
            _watchdog.RecordError($"EmployeeFullSyncJob failed. IsTransient={OracleHcmTransientFaultDetector.IsTransient(ex)} ExceptionType={ex.GetType().Name}");
            _logger.LogError(ex, "EmployeeFullSyncJob failed");
            throw new JobExecutionException("EmployeeFullSyncJob failed", ex, refireImmediately: false);
        }
        finally
        {
            _watchdog.RecordHeartbeat();
        }
    }
}
