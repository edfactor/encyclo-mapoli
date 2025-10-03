using System.Diagnostics;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal sealed class EmployeeFullSyncService : OracleHcmHostedServiceBase
{
    public EmployeeFullSyncService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig,
        ILogger<EmployeeFullSyncService> logger) : base(schedulerFactory, jobFactory, oracleHcmConfig, logger)
    {

    }

    protected override Task ConfigureJob(CancellationToken cancellationToken)
    {
        return ScheduleJob<EmployeeFullSyncJob>(
            "employeeFullSyncTrigger",
            Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(2),
            TimeSpan.FromHours(OracleHcmConfig.IntervalInHours),
            cancellationToken
        );
    }
}
