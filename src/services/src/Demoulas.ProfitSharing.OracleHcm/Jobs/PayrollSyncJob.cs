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

    public PayrollSyncJob(PayrollSyncClient payrollSyncClient)
    {
        _payrollSyncClient = payrollSyncClient;
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _payrollSyncClient.RetrievePayrollBalancesAsync(requestedBy: Constants.SystemAccountName, context.CancellationToken);
    }
}
