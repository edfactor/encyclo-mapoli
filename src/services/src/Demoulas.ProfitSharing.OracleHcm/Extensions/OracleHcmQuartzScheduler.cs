using Demoulas.ProfitSharing.OracleHcm.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;
public static class OracleHcmQuartzScheduler
{
    public static void AddOracleHcmQuartzServices(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("OracleHcmSyncJob");

            q.AddJob<OracleHcmJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("OracleHcmJobTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromHours(24))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
