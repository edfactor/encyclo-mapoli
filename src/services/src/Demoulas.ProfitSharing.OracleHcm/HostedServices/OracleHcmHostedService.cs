using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

internal abstract class OracleHcmHostedServiceBase : IHostedService
{
    protected OracleHcmConfig OracleHcmConfig { get; }

    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly ILogger _logger;
    private IScheduler? _scheduler;

    protected OracleHcmHostedServiceBase(ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        OracleHcmConfig oracleHcmConfig,
        ILogger logger)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _logger = logger;
        OracleHcmConfig = oracleHcmConfig;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!OracleHcmConfig.EnableSync)
        {
            return;
        }

        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
        _scheduler.JobFactory = _jobFactory;

        // Schedule all jobs

        await ConfigureJob(cancellationToken).ConfigureAwait(false);

        await _scheduler.Start(cancellationToken).ConfigureAwait(false);
    }

    protected abstract Task ConfigureJob(CancellationToken cancellationToken);

    protected Task ScheduleJob<TJob>(
        string triggerIdentity,
        TimeSpan startDelay,
        TimeSpan interval,
        CancellationToken cancellationToken
    ) where TJob : IJob
    {
        IJobDetail job = JobBuilder.Create<TJob>()
            .WithIdentity(typeof(TJob).Name)
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(triggerIdentity)
            .StartAt(DateTimeOffset.UtcNow.Add(startDelay))
            .WithSimpleSchedule(x => x
                .WithInterval(interval)
                .RepeatForever()
            )
            .Build();
        _logger.LogInformation("Scheduling Job {Job} with interval of {Interval}", typeof(TJob).Name, interval);
        return _scheduler!.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown(cancellationToken).ConfigureAwait(false);
        }
    }
}
