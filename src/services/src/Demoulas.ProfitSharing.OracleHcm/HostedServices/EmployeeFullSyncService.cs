using System.Diagnostics;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal sealed class EmployeeFullSyncService : OracleHcmHostedServiceBase
{
    public EmployeeFullSyncService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig) : base(schedulerFactory, jobFactory, oracleHcmConfig)
    {
       
    }
    
    protected override Task ConfigureJob(CancellationToken cancellationToken)
    {
        return ScheduleJob<EmployeeFullSyncJob>(
            "employeeFullSyncTrigger",
            Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(15),
            TimeSpan.FromHours(OracleHcmConfig.IntervalInHours),
            cancellationToken
        );
    }
}
