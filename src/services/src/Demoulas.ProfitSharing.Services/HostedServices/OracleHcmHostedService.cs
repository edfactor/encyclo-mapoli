using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Services.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public class OracleHcmHostedService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private IScheduler? _scheduler;

    public OracleHcmHostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;

        // Run the initial task
        await RunStartupTask();

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

    private Task RunStartupTask()
    {
        // Your startup task logic here
        Console.WriteLine("Running startup task");
        return Task.CompletedTask;
    }
}
