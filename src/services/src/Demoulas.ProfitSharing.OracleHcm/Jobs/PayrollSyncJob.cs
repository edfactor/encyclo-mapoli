using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

/// <summary>
/// Represents a job that synchronizes payroll data with the Oracle HCM system.
/// This job is scheduled and executed by the Quartz.NET scheduler.
/// </summary>
internal sealed class PayrollSyncJob : IJob
{
    private readonly PayrollSyncClient _payrollSyncClient;
    private readonly IProcessWatchdog _watchdog;

    public PayrollSyncJob(PayrollSyncClient payrollSyncClient, IProcessWatchdog watchdog)
    {
        _payrollSyncClient = payrollSyncClient;
        _watchdog = watchdog;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _payrollSyncClient.RetrievePayrollBalancesAsync(requestedBy: Constants.SystemAccountName, context.CancellationToken).ConfigureAwait(false);
            _watchdog.RecordSuccessfulCycle();
            _watchdog.RecordHeartbeat();
        }
        catch (Exception ex)
        {
            _watchdog.RecordError($"PayrollSyncJob failed: {ex.Message}");
            throw;
        }
    }
}
