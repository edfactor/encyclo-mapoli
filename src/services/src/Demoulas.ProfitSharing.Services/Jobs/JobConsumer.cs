using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit.Contracts.JobService;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using static Quartz.Logging.OperationName;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Job = Demoulas.ProfitSharing.Data.Entities.MassTransit.Job;
using JobStatus = Demoulas.ProfitSharing.Data.Entities.MassTransit.JobStatus;

namespace Demoulas.ProfitSharing.Services.Jobs;

public class JobConsumer : IConsumer<MessageRequest<OracleHcmJobRequest>>
{
    private readonly IProfitSharingDataContextFactory _dataContext;
    private readonly EmployeeSyncJob _employeeSyncJob;

    public JobConsumer(IProfitSharingDataContextFactory context, EmployeeSyncJob employeeSyncJob)
    {
        _dataContext = context;
        _employeeSyncJob = employeeSyncJob;
    }

    public async Task Consume(ConsumeContext<MessageRequest<OracleHcmJobRequest>> context)
    {
        _ = OracleHcmActivitySource.Instance.StartActivity(name: "Sync Employees from OracleHCM - Message received", kind: ActivityKind.Internal);
        CancellationToken cancellationToken = context.CancellationToken;
        var message = context.Message;

        if (message.Body.JobType is OracleHcmJobRequest.Enum.JobTypeEnum.Full or OracleHcmJobRequest.Enum.JobTypeEnum.Delta)
        {
            bool jobIsAlreadyRunning = await _dataContext.UseReadOnlyContext(c =>
            {
                var runningJobs = c.Jobs
                    .Where(j => (j.JobType == OracleHcmJobRequest.Enum.JobTypeEnum.Full || j.JobType == OracleHcmJobRequest.Enum.JobTypeEnum.Delta) &&
                                j.JobStatusId == JobStatus.Constants.Running);

                return runningJobs.AnyAsync(cancellationToken: cancellationToken);
            });

            if (jobIsAlreadyRunning)
            {
                _ = OracleHcmActivitySource.Instance.StartActivity(name: "Sync Employees from OracleHCM - Job already running. Exiting", kind: ActivityKind.Internal);
                return;
                
            }

            _= OracleHcmActivitySource.Instance.StartActivity(name: $"Sync Employees from OracleHCM - Start new {message.Body.JobType} sync job", kind: ActivityKind.Internal);
            var job = new Job
            {
                JobType = message.Body.JobType,
                StartMethod = message.Body.StartMethod,
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
            await ExecuteJob(cancellationToken);

            await _dataContext.UseWritableContext(c =>
            {
                job.Completed = DateTime.Now;
                job.JobStatusId = JobStatus.Constants.Completed;
                return c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

        }
    }

    private Task ExecuteJob(CancellationToken cancellationToken)
    {
        return _employeeSyncJob.SynchronizeEmployees(cancellationToken);
    }
}


