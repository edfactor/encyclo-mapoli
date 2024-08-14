using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Microsoft.Extensions.Logging;
using Job = Demoulas.ProfitSharing.Data.Entities.MassTransit.Job;
using JobStatus = Demoulas.ProfitSharing.Data.Entities.MassTransit.JobStatus;

namespace Demoulas.ProfitSharing.Services.Jobs;

public class JobConsumer : IConsumer<MessageRequest<OracleHcmJobRequest>>
{
    private readonly IProfitSharingDataContextFactory _dataContext;
    private readonly IEmployeeSyncJob _employeeSyncJob;
    private readonly ILogger<JobConsumer> _logger;

    public JobConsumer(IProfitSharingDataContextFactory context, 
        IEmployeeSyncJob employeeSyncJob,
        ILogger<JobConsumer> logger)
    {
        _dataContext = context;
        _employeeSyncJob = employeeSyncJob;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MessageRequest<OracleHcmJobRequest>> context)
    {
        _ = OracleHcmActivitySource.Instance.StartActivity(name: "Sync Employees from OracleHCM - Message received", kind: ActivityKind.Internal);
        CancellationToken cancellationToken = context.CancellationToken;
        var message = context.Message;

        if (message.Body.JobType is JobType.Constants.Full or JobType.Constants.Delta)
        {
            bool jobIsAlreadyRunning = await _dataContext.UseReadOnlyContext(c =>
            {
                var runningJobs = c.Jobs
                    .Where(j => (j.JobTypeId == JobType.Constants.Full || j.JobTypeId == JobType.Constants.Delta) &&
                                j.JobStatusId == JobStatus.Constants.Running
                                && j.Started > DateTime.Now.AddDays(-1));

                return runningJobs.AnyAsync(cancellationToken: cancellationToken);
            });

            if (jobIsAlreadyRunning)
            {
                _logger.LogWarning("Sync Employees from OracleHCM - Job [{message}] already running. Exiting", message);
                return;
            }

            _= OracleHcmActivitySource.Instance.StartActivity(name: $"Sync Employees from OracleHCM - Start new {message.Body.JobType} sync job", kind: ActivityKind.Internal);
            var job = new Job
            {
                JobTypeId = message.Body.JobType,
                StartMethodId = StartMethod.Constants.System,
                RequestedBy = message.Body.RequestedBy,
                JobStatusId = JobStatus.Constants.Running,
                Started = DateTime.Now
            };

            await _dataContext.UseWritableContext(c =>
            {
                c.Jobs.Add(job);
                return c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);


            // Execute the job
            _logger.LogWarning("Sync Employees from OracleHCM - Start new job : {job}", job);
            await ExecuteJob(cancellationToken);

            await _dataContext.UseWritableContext(c =>
            {
                job.Completed = DateTime.Now;
                job.JobStatusId = JobStatus.Constants.Completed;
                return c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            _logger.LogWarning("Sync Employees from OracleHCM - Completed job : {job}", job);

        }
    }

    private Task ExecuteJob(CancellationToken cancellationToken)
    {
        return _employeeSyncJob.SynchronizeEmployees(cancellationToken);
    }
}


