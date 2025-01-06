using System.Diagnostics;
using System.Runtime.CompilerServices;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using FluentValidation.Results;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Service responsible for synchronizing employee data from the Oracle HCM system to the Profit Sharing system.
/// This includes fetching employee data, validating it, and updating the Profit Sharing database.
/// </summary>
internal sealed class EmployeeSyncService : IEmployeeSyncService
{
    private readonly OracleEmployeeDataSyncClient _oracleEmployeeDataSyncClient;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly AtomFeedClient _atomFeedClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly IBus _employeeSyncBus;

    public EmployeeSyncService(AtomFeedClient atomFeedClient,
        OracleEmployeeDataSyncClient oracleEmployeeDataSyncClient,
        IDemographicsServiceInternal demographicsService,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        IBus employeeSyncBus)
    {
        _oracleEmployeeDataSyncClient = oracleEmployeeDataSyncClient;
        _demographicsService = demographicsService;
        _atomFeedClient = atomFeedClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _employeeSyncBus = employeeSyncBus;
    }

    public async Task SynchronizeEmployeesAsync(string requestedBy = "System", CancellationToken cancellationToken = default)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployeesAsync), ActivityKind.Internal);

        var job = new Job
        {
            JobTypeId = JobType.Constants.EmployeeSyncFull,
            StartMethodId = StartMethod.Constants.System,
            RequestedBy = requestedBy,
            JobStatusId = JobStatus.Constants.Running,
            Started = DateTime.Now
        };

        await _profitSharingDataContextFactory.UseWritableContext(db =>
        {
            db.Jobs.Add(job);
            return db.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        bool success = true;
        try
        {
            await _demographicsService.CleanAuditError(cancellationToken);
            var oracleHcmEmployees = _oracleEmployeeDataSyncClient.GetAllEmployees(cancellationToken);
            await QueueEmployee(requestedBy, cancellationToken, oracleHcmEmployees);
        }
        catch (Exception ex)
        {
            success = false;
            await _demographicsService.AuditError(0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken);
        }
        finally
        {
            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j => j.Id == job.Id).ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.Completed, b => DateTime.Now)
                        .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken);
        }
    }

    public async Task ExecuteDeltaSyncAsync(string requestedBy = "System", CancellationToken cancellationToken = default)
    {
        try
        {
            var maxDate = DateTime.Now;
            var minDate = await _profitSharingDataContextFactory.UseReadOnlyContext(c =>
            {
                return c.Demographics.MinAsync(d => d.LastModifiedDate - TimeSpan.FromDays(30), cancellationToken: cancellationToken);
            });

            var newHires = _atomFeedClient.GetFeedDataAsync<NewHireContext>("newhire", minDate, maxDate, cancellationToken);
            var assignments = _atomFeedClient.GetFeedDataAsync<AssignmentContext>("empassignment", minDate, maxDate, cancellationToken);
            var updates = _atomFeedClient.GetFeedDataAsync<EmployeeUpdateContext>("empupdate", minDate, maxDate, cancellationToken);
            var terminations = _atomFeedClient.GetFeedDataAsync<TerminationContext>("termination", minDate, maxDate, cancellationToken);

            HashSet<long> people = new HashSet<long>();
            await foreach (var record in MergeAsyncEnumerables(newHires, updates, terminations, assignments, cancellationToken))
            {
                people.Add(record.PersonId);
            }

            try
            {
                foreach (long oracleHcmId in people)
                {
                    var oracleHcmEmployees = _oracleEmployeeDataSyncClient.GetEmployee(oracleHcmId, cancellationToken);
                    await QueueEmployee(requestedBy, cancellationToken, oracleHcmEmployees);
                }
            }
            catch (Exception ex)
            {
                await _demographicsService.AuditError(0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken);
            }

        }
        catch (Exception ex)
        {
            await _demographicsService.AuditError(0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken);
        }
    }

    private async Task QueueEmployee(string requestedBy, CancellationToken cancellationToken, IAsyncEnumerable<OracleEmployee?> oracleHcmEmployees)
    {
        await foreach (var employee in oracleHcmEmployees.WithCancellation(cancellationToken))
        {
            if (employee == null)
            {
                continue;
            }

            var message = new MessageRequest<OracleEmployee>
            {
                ApplicationName = nameof(EmployeeSyncService), Body = employee, UserId = requestedBy
            };
                        
            await _employeeSyncBus.Publish<MessageRequest<OracleEmployee>>(message, cancellationToken);
        }
    }


    /// <summary>
    /// Merges multiple asynchronous enumerables of <see cref="T"/> into a single asynchronous enumerable.
    /// </summary>
    /// <param name="first">The first asynchronous enumerable to merge.</param>
    /// <param name="second">The second asynchronous enumerable to merge.</param>
    /// <param name="third">The third asynchronous enumerable to merge.</param>
    /// <param name="fourth">The fourth asynchronous enumerable to merge.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An asynchronous enumerable that yields elements from all provided enumerables in sequence.
    /// </returns>
    private static async IAsyncEnumerable<DeltaContextBase> MergeAsyncEnumerables(IAsyncEnumerable<DeltaContextBase> first,
        IAsyncEnumerable<DeltaContextBase> second,
        IAsyncEnumerable<DeltaContextBase> third,
        IAsyncEnumerable<DeltaContextBase> fourth,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in first.WithCancellation(cancellationToken))
        {
            yield return item;
        }

        await foreach (var item in second.WithCancellation(cancellationToken))
        {
            yield return item;
        }

        await foreach (var item in third.WithCancellation(cancellationToken))
        {
            yield return item;
        }

        await foreach (var item in fourth.WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }
}
