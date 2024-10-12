using Demoulas.ProfitSharing.Common.Interfaces;
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
    private IScheduler? _scheduler;

    public OracleHcmHostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory,
        IServiceProvider serviceProvider)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _serviceProvider = serviceProvider;
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

        var trigger = TriggerBuilder.Create()
            .WithIdentity("dailyTrigger")
            .WithCronSchedule("0 0 0 * * ?") // Runs daily at midnight
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

