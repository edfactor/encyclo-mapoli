using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;
internal sealed class EmployeeSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly IProfitSharingDataContextFactory _factory;

    public EmployeeSyncJob(IEmployeeSyncService employeeSyncService, IProfitSharingDataContextFactory factory)
    {
        _employeeSyncService = employeeSyncService;
        _factory = factory;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var job = new Job
        {
            JobTypeId = JobType.Constants.Full,
            StartMethodId = StartMethod.Constants.System,
            RequestedBy = "System",
            JobStatusId = JobStatus.Constants.Running,
            Started = DateTime.Now
        };

        _factory.UseWritableContext(async db =>
        {
            db.Jobs.Add(job);
            await db.SaveChangesAsync(context.CancellationToken);
        }, context.CancellationToken);

        return _employeeSyncService.SynchronizeEmployees(context.CancellationToken);
    }
}
