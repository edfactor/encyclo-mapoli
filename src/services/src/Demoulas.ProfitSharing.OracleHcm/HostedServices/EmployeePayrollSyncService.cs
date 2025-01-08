using System.Diagnostics;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal sealed class EmployeePayrollSyncService : OracleHcmHostedServiceBase
{
    public EmployeePayrollSyncService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig) : base(schedulerFactory, jobFactory, oracleHcmConfig)
    {
       
    }
    
    protected override Task ConfigureJob(CancellationToken cancellationToken)
    {
        return ScheduleJob<PayrollSyncJob>(
            "payrollSyncTrigger",
            Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(OracleHcmConfig.IntervalInHours),
            cancellationToken
        );
    }
}
