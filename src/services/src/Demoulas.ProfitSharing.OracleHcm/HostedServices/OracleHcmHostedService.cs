using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Microsoft.Extensions.DependencyInjection;
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
        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;

        // Schedule the recurring job for EmployeeSyncJob
        var employeeSyncJob = JobBuilder.Create<EmployeeSyncJob>()
            .WithIdentity(nameof(EmployeeSyncJob))
            .Build();

        var employeeSyncTrigger = TriggerBuilder.Create()
            .WithIdentity("employeeSyncTrigger")
            .StartNow()
            .WithSimpleSchedule(x =>
            {
                x.WithIntervalInHours(_oracleHcmConfig.IntervalInHours)
                    .RepeatForever();
            })
            .Build();

        // Schedule the recurring job for PayrollSyncJob
        var payrollSyncJob = JobBuilder.Create<PayrollSyncJob>()
            .WithIdentity(nameof(PayrollSyncJob))
            .Build();

        var payrollSyncTrigger = TriggerBuilder.Create()
            .WithIdentity("payrollSyncTrigger")
            .StartAt(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithSimpleSchedule(x =>
            {
                x.WithIntervalInHours(_oracleHcmConfig.IntervalInHours)
                    .RepeatForever();
            })
            .Build();

        await _scheduler.ScheduleJob(employeeSyncJob, employeeSyncTrigger, cancellationToken);
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
