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

        // Attach to unhandled exception events
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
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

        // Detach unhandled exception handlers
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
    }

    /*
     * https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#avoid-async-void
     * To summarize this first guideline, you should prefer async Task to async void.
     * Async Task methods enable easier error-handling, composability and testability.
     *
     * The exception to this guideline is asynchronous event handlers, which must return void.
     * This exception includes methods that are logically event handlers even if they’re not literally event handlers (for example, ICommand.Execute implementations).
     */
#pragma warning disable VSTHRD100
    private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        await HandleException(e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception."));
    }

    private async void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        await HandleException(e.Exception);
    }
#pragma warning restore VSTHRD100

    private async Task HandleException(Exception exception)
    {
        // Log the exception
        _logger.LogCritical(exception, "Unhandled exception: {Exception}", exception);

        // Stop the service
        await StopAsync(CancellationToken.None);

        // Terminate the process
        Environment.Exit(-1);
    }
}
