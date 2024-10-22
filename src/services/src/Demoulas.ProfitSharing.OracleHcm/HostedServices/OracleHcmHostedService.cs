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
    private readonly IServiceProvider _serviceProvider;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private IScheduler? _scheduler;

    public OracleHcmHostedService(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        IServiceProvider serviceProvider,
        OracleHcmConfig oracleHcmConfig)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _serviceProvider = serviceProvider;
        _oracleHcmConfig = oracleHcmConfig;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;

        // Run the initial task ( Fire and forget )
        _ = RunStartupTask(cancellationToken);

        // Schedule the recurring job
        var job = JobBuilder.Create<EmployeeSyncJob>()
            .WithIdentity("dailyJob")
            .Build();

        /*
         * Explanation of the cron expression:
            0 0 0/4 1/1 * ? *
            0: Seconds (0th second)
            0: Minutes (0th minute)
            0/4: Hours (every 4 hours, starting from midnight)
            1/1: Day of month (every day)
            *: Month (every month)
            ?: Day of week (no specific day of the week)
            *: Year (every year)
         */
        var trigger = TriggerBuilder.Create()
            .WithIdentity("dailyTrigger")
            .WithCronSchedule(_oracleHcmConfig.CronSchedule, builder =>
            {
                builder.InTimeZone(TimeZoneInfo.Local);
            }) // Runs daily at midnight
            .Build();

        await _scheduler.ScheduleJob(job, trigger, cancellationToken);

        await _scheduler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }

    private Task RunStartupTask(CancellationToken cancellationToken)
    {
        var employeeSyncService = _serviceProvider.GetRequiredService<IEmployeeSyncService>();
        return employeeSyncService.SynchronizeEmployees(cancellationToken);
    }
}

