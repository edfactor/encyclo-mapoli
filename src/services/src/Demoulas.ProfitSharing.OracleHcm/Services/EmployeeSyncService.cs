using System.Diagnostics;
using System.Runtime;
using System.Threading.Channels;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Job = Demoulas.ProfitSharing.Data.Entities.Scheduling.Job;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Service responsible for synchronizing employee data from the Oracle HCM system to the Profit Sharing system.
/// This includes fetching employee data, validating it, and updating the Profit Sharing database.
/// </summary>
internal sealed class EmployeeSyncService : IEmployeeSyncService
{
    private readonly EmployeeFullSyncClient _oracleEmployeeDataSyncClient;
    private readonly AtomFeedClient _atomFeedClient;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly Channel<MessageRequest<OracleEmployee[]>> _employeeChannel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EmployeeSyncService> _logger;

    public EmployeeSyncService(AtomFeedClient atomFeedClient,
        EmployeeFullSyncClient oracleEmployeeDataSyncClient,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        Channel<MessageRequest<OracleEmployee[]>> employeeChannel,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EmployeeSyncService> logger)
    {
        _oracleEmployeeDataSyncClient = oracleEmployeeDataSyncClient;
        _atomFeedClient = atomFeedClient;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _employeeChannel = employeeChannel;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task ExecuteFullSyncAsync(string requestedBy = Constants.SystemAccountName, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using Activity? activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ExecuteFullSyncAsync), ActivityKind.Internal);

        // Add telemetry tags
        activity?.SetTag("sync.type", "full");
        activity?.SetTag("requested.by", requestedBy);
        activity?.SetTag("operation", "employee-full-sync");

        using var scope = _serviceScopeFactory.CreateScope();
        var demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();

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
        int employeeBatchesProcessed = 0;
        try
        {
            await demographicsService.CleanAuditError(cancellationToken).ConfigureAwait(false);
            await foreach (OracleEmployee[] oracleHcmEmployees in _oracleEmployeeDataSyncClient.GetAllEmployees(cancellationToken).ConfigureAwait(false))
            {
                await QueueEmployee(requestedBy, oracleHcmEmployees, cancellationToken).ConfigureAwait(false);
                employeeBatchesProcessed++;
            }

            // Record success metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employee-full-sync"),
                new("status", "success"),
                new("service", nameof(EmployeeSyncService)));

            activity?.SetTag("batches.processed", employeeBatchesProcessed);
            activity?.SetTag("status", "success");
        }
        catch (Exception ex)
        {
            success = false;
            await demographicsService.AuditError(0, 0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken).ConfigureAwait(false);

            // Record failure metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employee-full-sync"),
                new("status", "failed"),
                new("service", nameof(EmployeeSyncService)));

            EndpointTelemetry.EndpointErrorsTotal.Add(1,
                new("error.type", ex.GetType().Name),
                new("operation", "employee-full-sync"),
                new("service", nameof(EmployeeSyncService)));

            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);

            _logger.LogError(ex, "Employee full sync failed after processing {BatchCount} batches", employeeBatchesProcessed);
        }
        finally
        {
            stopwatch.Stop();

            // Record duration metrics
            EndpointTelemetry.BusinessLogicDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds,
                new("operation", "employee-full-sync"),
                new("service", nameof(EmployeeSyncService)));

            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j => j.Id == job.Id).ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.Completed, b => DateTime.Now)
                        .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Employee full sync completed. Success: {Success}, Batches: {BatchCount}, Duration: {DurationMs}ms",
                success, employeeBatchesProcessed, stopwatch.Elapsed.TotalMilliseconds);

#pragma warning disable S1215
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
#pragma warning restore S1215
        }
    }

    public async Task ExecuteDeltaSyncAsync(string requestedBy = Constants.SystemAccountName, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        using Activity? activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ExecuteDeltaSyncAsync), ActivityKind.Internal);

        // Add telemetry tags
        activity?.SetTag("sync.type", "delta");
        activity?.SetTag("requested.by", requestedBy);
        activity?.SetTag("operation", "employee-delta-sync");

        using var scope = _serviceScopeFactory.CreateScope();
        var demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();

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
        int totalChangedEmployees = 0;
        try
        {
            DateTimeOffset maxDate = DateTimeOffset.UtcNow;
            DateTimeOffset minDate = await _profitSharingDataContextFactory.UseReadOnlyContext(c =>
            {
                return c.Demographics
                    .TagWith($"DeltaSync-GetMinModifiedDate")
                    .MinAsync(d => (d.ModifiedAtUtc == null ? d.CreatedAtUtc : d.ModifiedAtUtc.Value) - TimeSpan.FromDays(7), cancellationToken: cancellationToken);
            }, cancellationToken).ConfigureAwait(false);

            // Fetch all feeds in parallel for better performance
            var newHiresTask = MaterializeAsync(_atomFeedClient.GetFeedDataAsync<NewHireContext>("newhire", minDate, maxDate, cancellationToken), cancellationToken);
            var assignmentsTask = MaterializeAsync(_atomFeedClient.GetFeedDataAsync<AssignmentContext>("empassignment", minDate, maxDate, cancellationToken), cancellationToken);
            var updatesTask = MaterializeAsync(_atomFeedClient.GetFeedDataAsync<EmployeeUpdateContext>("empupdate", minDate, maxDate, cancellationToken), cancellationToken);
            var terminationsTask = MaterializeAsync(_atomFeedClient.GetFeedDataAsync<TerminationContext>("termination", minDate, maxDate, cancellationToken), cancellationToken);

            await Task.WhenAll(newHiresTask, assignmentsTask, updatesTask, terminationsTask).ConfigureAwait(false);

            // Get results from completed tasks
            var newHires = await newHiresTask.ConfigureAwait(false);
            var assignments = await assignmentsTask.ConfigureAwait(false);
            var updates = await updatesTask.ConfigureAwait(false);
            var terminations = await terminationsTask.ConfigureAwait(false);

            // Merge results and extract unique person IDs
            HashSet<long> people = new HashSet<long>();
            foreach (var record in newHires.Cast<DeltaContextBase>()
                .Concat(assignments)
                .Concat(updates)
                .Concat(terminations))
            {
                people.Add(record.PersonId);
            }

            totalChangedEmployees = people.Count;

            // Record feed-specific metrics
            activity?.SetTag("feed.newhires", newHires.Count);
            activity?.SetTag("feed.assignments", assignments.Count);
            activity?.SetTag("feed.updates", updates.Count);
            activity?.SetTag("feed.terminations", terminations.Count);
            activity?.SetTag("employees.changed", totalChangedEmployees);

            EndpointTelemetry.RecordCountsProcessed.Record(totalChangedEmployees,
                new("operation", "employee-delta-sync"),
                new("record.type", "changed-employees"),
                new("service", nameof(EmployeeSyncService)));

            await TrySyncEmployeeFromOracleHcm(requestedBy, people, cancellationToken).ConfigureAwait(false);

            // Record success metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employee-delta-sync"),
                new("status", "success"),
                new("service", nameof(EmployeeSyncService)));

            activity?.SetTag("status", "success");

        }
        catch (Exception ex)
        {
            success = false;
            await demographicsService.AuditError(0, 0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken).ConfigureAwait(false);

            // Record failure metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employee-delta-sync"),
                new("status", "failed"),
                new("service", nameof(EmployeeSyncService)));

            EndpointTelemetry.EndpointErrorsTotal.Add(1,
                new("error.type", ex.GetType().Name),
                new("operation", "employee-delta-sync"),
                new("service", nameof(EmployeeSyncService)));

            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);

            _logger.LogError(ex, "Employee delta sync failed. Changed employees: {EmployeeCount}", totalChangedEmployees);
        }
        finally
        {
            stopwatch.Stop();

            // Record duration metrics
            EndpointTelemetry.BusinessLogicDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds,
                new("operation", "employee-delta-sync"),
                new("service", nameof(EmployeeSyncService)));

            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j => j.Id == job.Id).ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.Completed, b => DateTime.Now)
                        .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Employee delta sync completed. Success: {Success}, Changed Employees: {EmployeeCount}, Duration: {DurationMs}ms",
                success, totalChangedEmployees, stopwatch.Elapsed.TotalMilliseconds);
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
            using var scope = _serviceScopeFactory.CreateScope();
            var demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();

            await demographicsService.AuditError(0, 0, [new ValidationFailure("Error", ex.Message)], requestedBy, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask QueueEmployee(string requestedBy, OracleEmployee[] employees, CancellationToken cancellationToken)
    {
        MessageRequest<OracleEmployee[]> message = new MessageRequest<OracleEmployee[]>
        {
            ApplicationName = nameof(EmployeeSyncService),
            Body = employees,
            UserId = requestedBy
        };

        return _employeeChannel.Writer.WriteAsync(message, cancellationToken);
    }

    /// <summary>
    /// Materializes an IAsyncEnumerable into a List asynchronously.
    /// </summary>
    private static async Task<List<T>> MaterializeAsync<T>(IAsyncEnumerable<T> source, CancellationToken cancellationToken)
    {
        var result = new List<T>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            result.Add(item);
        }
        return result;
    }
}
