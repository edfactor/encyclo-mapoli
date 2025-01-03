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

        // Schedule all jobs

        //await ScheduleJob<EmployeeDeltaSyncJob>(
        //    "employeeDeltaSyncTrigger",
        //    Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(5),
        //    TimeSpan.FromMinutes(_oracleHcmConfig.DeltaIntervalInMinutes),
        //    cancellationToken
#pragma warning disable S125
        //);
#pragma warning restore S125

        //await ScheduleJob<EmployeeFullSyncJob>(
        //    "employeeFullSyncTrigger",
        //    Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(15),
        //    TimeSpan.FromHours(_oracleHcmConfig.IntervalInHours),
        //    cancellationToken
#pragma warning disable S125
        //);
#pragma warning restore S125


        await ScheduleJob<PayrollSyncJob>(
            "payrollSyncTrigger",
            Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(_oracleHcmConfig.IntervalInHours),
            cancellationToken
#pragma warning disable S125
        );
#pragma warning restore S125

        await _scheduler.Start(cancellationToken);
    }

    private Task ScheduleJob<TJob>(
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
