using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal abstract class OracleHcmHostedServiceBase : IHostedService
{
    protected OracleHcmConfig OracleHcmConfig { get; }
    
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private IScheduler? _scheduler;

    protected OracleHcmHostedServiceBase(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        OracleHcmConfig = oracleHcmConfig;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!OracleHcmConfig.EnableSync)
        {
            return;
        }

        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;

        // Schedule all jobs

        await ConfigureJob(cancellationToken);

        await _scheduler.Start(cancellationToken);
    }

    protected abstract Task ConfigureJob(CancellationToken cancellationToken);

    protected Task ScheduleJob<TJob>(
        string triggerIdentity,
        TimeSpan startDelay,
        TimeSpan interval,
        CancellationToken cancellationToken
    ) where TJob : IJob
    {
        var job = JobBuilder.Create<TJob>()
            .WithIdentity(typeof(TJob).Name)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerIdentity)
            .StartAt(DateTimeOffset.UtcNow.Add(startDelay))
            .WithSimpleSchedule(x => x
                .WithInterval(interval)
                .RepeatForever()
            )
            .Build();

        return _scheduler!.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
