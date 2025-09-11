using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Job = Demoulas.ProfitSharing.Data.Entities.Scheduling.Job;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Service responsible for synchronizing employee data from the Oracle HCM system to the Profit Sharing system.
/// This includes fetching employee data, validating it, and updating the Profit Sharing database.
/// </summary>
internal sealed class EmployeeSyncService : IEmployeeSyncService
{
    private readonly EmployeeFullSyncClient _oracleEmployeeDataSyncClient;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly AtomFeedClient _atomFeedClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly Channel<MessageRequest<OracleEmployee[]>> _employeeChannel;

    public EmployeeSyncService(AtomFeedClient atomFeedClient,
        EmployeeFullSyncClient oracleEmployeeDataSyncClient,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        Channel<MessageRequest<OracleEmployee[]>> employeeChannel,
        IServiceScopeFactory serviceScopeFactory)
    {
        _oracleEmployeeDataSyncClient = oracleEmployeeDataSyncClient;
        var scope = serviceScopeFactory.CreateScope();
        _demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();
        _atomFeedClient = atomFeedClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;

        _employeeChannel = employeeChannel;
    }

    public async Task ExecuteFullSyncAsync(string requestedBy = Constants.SystemAccountName, CancellationToken cancellationToken = default)
    {
        using Activity? activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ExecuteFullSyncAsync), ActivityKind.Internal);

        Job job = new Job
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
        }, cancellationToken).ConfigureAwait(false);

        bool success = true;
        try
        {
            await _demographicsService.CleanAuditError(cancellationToken).ConfigureAwait(false);
            await foreach (OracleEmployee[] oracleHcmEmployees in _oracleEmployeeDataSyncClient.GetAllEmployees(cancellationToken).ConfigureAwait(false) )
            {
                await QueueEmployee(requestedBy, oracleHcmEmployees, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            success = false;
            await _demographicsService.AuditError(0, 0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j => j.Id == job.Id).ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.Completed, b => DateTime.Now)
                        .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken).ConfigureAwait(false);

#pragma warning disable S1215
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
#pragma warning restore S1215
        }
    }

    public async Task ExecuteDeltaSyncAsync(string requestedBy = Constants.SystemAccountName, CancellationToken cancellationToken = default)
    {
        using Activity? activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ExecuteDeltaSyncAsync), ActivityKind.Internal);

        Job job = new Job
        {
            JobTypeId = JobType.Constants.EmployeeSyncDelta,
            StartMethodId = StartMethod.Constants.System,
            RequestedBy = requestedBy,
            JobStatusId = JobStatus.Constants.Running,
            Started = DateTimeOffset.UtcNow
        };

        await _profitSharingDataContextFactory.UseWritableContext(db =>
        {
            db.Jobs.Add(job);
            return db.SaveChangesAsync(cancellationToken);
        }, cancellationToken).ConfigureAwait(false);
        bool success = true;
        try
        {
            DateTimeOffset maxDate = DateTimeOffset.UtcNow;
            DateTimeOffset minDate = await _profitSharingDataContextFactory.UseReadOnlyContext(c =>
            {
                return c.Demographics.MinAsync(d => (d.ModifiedAtUtc == null ? d.CreatedAtUtc : d.ModifiedAtUtc.Value) - TimeSpan.FromDays(7), cancellationToken: cancellationToken);
            }).ConfigureAwait(false);

            IAsyncEnumerable<NewHireContext> newHires = _atomFeedClient.GetFeedDataAsync<NewHireContext>("newhire", minDate, maxDate, cancellationToken);
            IAsyncEnumerable<AssignmentContext> assignments = _atomFeedClient.GetFeedDataAsync<AssignmentContext>("empassignment", minDate, maxDate, cancellationToken);
            IAsyncEnumerable<EmployeeUpdateContext> updates = _atomFeedClient.GetFeedDataAsync<EmployeeUpdateContext>("empupdate", minDate, maxDate, cancellationToken);
            IAsyncEnumerable<TerminationContext> terminations = _atomFeedClient.GetFeedDataAsync<TerminationContext>("termination", minDate, maxDate, cancellationToken);

            HashSet<long> people = new HashSet<long>();
            await foreach (DeltaContextBase record in MergeAsyncEnumerables(newHires, updates, terminations, assignments, cancellationToken).ConfigureAwait(false))
            {
                people.Add(record.PersonId);
            }

            await TrySyncEmployeeFromOracleHcm(requestedBy, people, cancellationToken).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            success = false;
            await _demographicsService.AuditError(0, 0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j => j.Id == job.Id).ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.Completed, b => DateTime.Now)
                        .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task TrySyncEmployeeFromOracleHcm(string requestedBy, ISet<long> people, CancellationToken cancellationToken)
    {
        try
        {
            foreach (long oracleHcmId in people)
            {
                OracleEmployee[] oracleHcmEmployees = await _oracleEmployeeDataSyncClient.GetEmployee(oracleHcmId, cancellationToken).ConfigureAwait(false);
                await QueueEmployee(requestedBy, oracleHcmEmployees, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            await _demographicsService.AuditError(0, 0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask QueueEmployee(string requestedBy, OracleEmployee[] employees, CancellationToken cancellationToken)
    {
        MessageRequest<OracleEmployee[]> message = new MessageRequest<OracleEmployee[]>
        {
            ApplicationName = nameof(EmployeeSyncService), Body = employees, UserId = requestedBy
        };

       return _employeeChannel.Writer.WriteAsync(message, cancellationToken);
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
        await foreach (DeltaContextBase item in first.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }

        await foreach (DeltaContextBase item in second.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }

        await foreach (DeltaContextBase item in third.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }

        await foreach (DeltaContextBase item in fourth.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return item;
        }
    }
}
