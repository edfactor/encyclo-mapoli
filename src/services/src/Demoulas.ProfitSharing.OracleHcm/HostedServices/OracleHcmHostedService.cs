using System.Diagnostics;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal sealed class OracleHcmHostedService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private IScheduler? _scheduler;

    public OracleHcmHostedService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _oracleHcmConfig = oracleHcmConfig;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_oracleHcmConfig.EnableSync)
        {
            return;
        }

        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;

        #region Full Sync ( REST API )

        // Schedule the recurring job for EmployeeFullSyncJob
        var employeeFullSyncJob = JobBuilder.Create<EmployeeFullSyncJob>()
            .WithIdentity(nameof(EmployeeFullSyncJob))
            .Build();

        var employeeFullSyncTrigger = TriggerBuilder.Create()
            .WithIdentity("employeeFullSyncTrigger")
            .StartAt(DateTimeOffset.UtcNow.AddMinutes(Debugger.IsAttached ? 0 : 5))
            .WithSimpleSchedule(x =>
            {
                x.WithIntervalInHours(_oracleHcmConfig.IntervalInHours)
                    .RepeatForever();
            })
            .Build();

        #endregion

        #region Delta Sync ( ATOM Feed )

        // Schedule the recurring job for EmployeeFullSyncJob
        var employeeDeltaSyncJob = JobBuilder.Create<EmployeeDeltaSyncJob>()
            .WithIdentity(nameof(EmployeeDeltaSyncJob))
            .Build();

        var employeeDeltaSyncTrigger = TriggerBuilder.Create()
            .WithIdentity("employeeDeltaSyncTrigger")
            .StartAt(DateTimeOffset.UtcNow.AddMinutes(Debugger.IsAttached ? 0 : 5))
            .WithSimpleSchedule(x =>
            {
                x.WithIntervalInMinutes(_oracleHcmConfig.IntervalInHours)
                    .RepeatForever();
            })
            .Build();

        #endregion

        #region Payroll Sync ( REST API )

        // Schedule the recurring job for PayrollSyncJob
        var payrollSyncJob = JobBuilder.Create<PayrollSyncJob>()
            .WithIdentity(nameof(PayrollSyncJob))
            .Build();

        var payrollSyncTrigger = TriggerBuilder.Create()
            .WithIdentity("payrollSyncTrigger")
            .StartAt(DateTimeOffset.UtcNow.AddMinutes(Debugger.IsAttached ? 0 : 15))
            .WithSimpleSchedule(x =>
            {
                x.WithIntervalInHours(_oracleHcmConfig.IntervalInHours)
                    .RepeatForever();
            })
            .Build();

        #endregion

        await _scheduler.ScheduleJob(employeeFullSyncJob, employeeFullSyncTrigger, cancellationToken);
        await _scheduler.ScheduleJob(employeeDeltaSyncJob, employeeDeltaSyncTrigger, cancellationToken);
        await _scheduler.ScheduleJob(payrollSyncJob, payrollSyncTrigger, cancellationToken);


        await _scheduler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
