using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.Factories;

/// <summary>
/// Factory class responsible for creating instances of Oracle HCM jobs.
/// </summary>
public class OracleHcmJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public OracleHcmJobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob
               ?? throw new ArgumentException("Failed to resolve job");
    }

    public void ReturnJob(IJob job) { }
}
