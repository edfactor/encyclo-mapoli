using System.Diagnostics;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal sealed class EmployeePayrollSyncService : OracleHcmHostedServiceBase
{
    public EmployeePayrollSyncService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig,
        ILogger<EmployeePayrollSyncService> logger) : base(schedulerFactory, jobFactory, oracleHcmConfig, logger)
    {

    }

    protected override Task ConfigureJob(CancellationToken cancellationToken)
    {
        return ScheduleJob<PayrollSyncJob>(
            "payrollSyncTrigger",
            Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(4),
            TimeSpan.FromHours(OracleHcmConfig.PayrollIntervalInHours),
            cancellationToken
        );
    }
}
