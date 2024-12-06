using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JobStatus = Demoulas.ProfitSharing.Data.Entities.MassTransit.JobStatus;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;

public class OracleHcmMessageConsumer : IConsumer<MessageRequest<OracleHcmJobRequest>>
{
    private readonly IProfitSharingDataContextFactory _dataContext;
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly ILogger<OracleHcmMessageConsumer> _logger;

    public OracleHcmMessageConsumer(IProfitSharingDataContextFactory context,
        IEmployeeSyncService employeeSyncService,
        ILogger<OracleHcmMessageConsumer> logger)
    {
        _dataContext = context;
        _employeeSyncService = employeeSyncService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MessageRequest<OracleHcmJobRequest>> context)
    {
        CancellationToken cancellationToken = context.CancellationToken;
        var message = context.Message;

        if (message.Body.JobType is JobType.Constants.EmployeeSyncFull or JobType.Constants.PayrollSyncFull)
        {
            bool jobIsAlreadyRunning = await _dataContext.UseReadOnlyContext(c =>
            {
                var runningJobs = c.Jobs
                    .Where(j => (j.JobTypeId == JobType.Constants.EmployeeSyncFull || j.JobTypeId == JobType.Constants.PayrollSyncFull) &&
                                j.JobStatusId == JobStatus.Constants.Running
                                && j.Started > DateTime.Now.AddHours(-8));

                return runningJobs.AnyAsync(cancellationToken: cancellationToken);
            });

            if (jobIsAlreadyRunning)
            {
                _logger.LogWarning("Sync Employees from OracleHCM  Job [{Message}] already running. Exiting", message);
                return;
            }


            await _employeeSyncService.SynchronizeEmployeesAsync(message.Body.RequestedBy, cancellationToken);
        }
    }
}
