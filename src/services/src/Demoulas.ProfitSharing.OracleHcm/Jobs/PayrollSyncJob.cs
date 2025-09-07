using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.Common.Metrics;
using System.Diagnostics;
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

    public PayrollSyncJob(PayrollSyncClient payrollSyncClient)
    {
        _payrollSyncClient = payrollSyncClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        GlobalMeter.IncrementJobInflight();
        var sw = Stopwatch.StartNew();
        bool success = true;
        GlobalMeter.JobRunCount.Add(1, new KeyValuePair<string, object?>("job.name", nameof(PayrollSyncJob)));
        try
        {
            await _payrollSyncClient.RetrievePayrollBalancesAsync(requestedBy: Constants.SystemAccountName, context.CancellationToken).ConfigureAwait(false);
        }
        catch
        {
            success = false;
            GlobalMeter.JobRunFailures.Add(1, new KeyValuePair<string, object?>("job.name", nameof(PayrollSyncJob)), new KeyValuePair<string, object?>("outcome", "failure"));
            throw;
        }
        finally
        {
            sw.Stop();
            GlobalMeter.JobRunDurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("job.name", nameof(PayrollSyncJob)), new KeyValuePair<string, object?>("outcome", success ? "success" : "failure"));
            GlobalMeter.DecrementJobInflight();
        }
    }
}
