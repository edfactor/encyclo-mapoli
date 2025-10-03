using System.Diagnostics;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal sealed class EmployeeDeltaSyncService : OracleHcmHostedServiceBase
{
    public EmployeeDeltaSyncService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig,
        ILogger<EmployeeDeltaSyncService> logger) : base(schedulerFactory, jobFactory, oracleHcmConfig, logger)
    {

    }

    protected override Task ConfigureJob(CancellationToken cancellationToken)
    {
        return ScheduleJob<EmployeeDeltaSyncJob>(
            "employeeDeltaSyncTrigger",
            Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(3),
            TimeSpan.FromMinutes(OracleHcmConfig.DeltaIntervalInMinutes),
            cancellationToken
        );
    }
}
