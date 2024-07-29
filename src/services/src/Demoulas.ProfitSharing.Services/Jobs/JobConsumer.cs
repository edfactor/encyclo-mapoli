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
        CancellationToken cancellationToken = context.CancellationToken;
        var message = context.Message;

        if (message.Body.JobType is "Full" or "Delta")
        {
            bool jobIsAlreadyRunning = await _dataContext.UseReadOnlyContext(c =>
            {
                var runningJobs = c.Jobs
                    .Where(j => (j.JobType == "Full" || j.JobType == "Delta") && j.Status == Data.Entities.MassTransit.Job.JobStatus.Running);

                return runningJobs.AnyAsync(cancellationToken: cancellationToken);
            });

            if (jobIsAlreadyRunning)
            {
                {
                    return;
                }
            }

            var job = new Demoulas.ProfitSharing.Data.Entities.MassTransit.Job
            {
                JobType = message.Body.JobType,
                StartMethod = message.Body.StartMethod,
                RequestedBy = message.Body.RequestedBy,
                Status = Data.Entities.MassTransit.Job.JobStatus.Running,
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
                job.Status = Data.Entities.MassTransit.Job.JobStatus.Completed;
                return c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

        }
    }

    private Task ExecuteJob(CancellationToken cancellationToken)
    {
        return _employeeSyncJob.SynchronizeEmployees(cancellationToken);
    }
}


