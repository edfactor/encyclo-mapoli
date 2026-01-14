using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes payroll data with the Oracle HCM system.
/// This job is scheduled and executed by the Quartz.NET scheduler.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class PayrollSyncJob : IJob
{
    private readonly PayrollSyncClient _payrollSyncClient;
    private readonly IProcessWatchdog _watchdog;
    private readonly ILogger<PayrollSyncJob> _logger;

    public PayrollSyncJob(PayrollSyncClient payrollSyncClient, IProcessWatchdog watchdog, ILogger<PayrollSyncJob> logger)
    {
        _payrollSyncClient = payrollSyncClient;
        _watchdog = watchdog;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _watchdog.RecordHeartbeat();

        try
        {
            ResiliencePipeline pipeline = OracleHcmRetryPolicy.Create(nameof(PayrollSyncJob), _logger, _watchdog);

            ResilienceContext resilienceContext = ResilienceContextPool.Shared.Get(context.CancellationToken);
            try
            {
                await pipeline.ExecuteAsync(
                    async (ResilienceContext ctx) =>
                    {
                        CancellationToken ct = ctx.CancellationToken;
                        await _payrollSyncClient.RetrievePayrollBalancesAsync(requestedBy: Constants.SystemAccountName, ct).ConfigureAwait(false);
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
            _watchdog.RecordError($"PayrollSyncJob failed. IsTransient={OracleHcmTransientFaultDetector.IsTransient(ex)} ExceptionType={ex.GetType().Name}");
            _logger.LogError(ex, "PayrollSyncJob failed");
            throw new JobExecutionException("PayrollSyncJob failed", ex, refireImmediately: false);
        }
        finally
        {
            _watchdog.RecordHeartbeat();
        }
    }
}
