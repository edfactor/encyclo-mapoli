using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Demoulas.ProfitSharing.Services;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public class SyncJobManager
{
    private readonly AtomFeedService _atomFeedService;
    private readonly DemographicsService _demographicsService;
    private readonly ILogger<SyncJobManager> _logger;
    private static readonly object _syncLock = new object();
    private bool _isFullSyncRunning;

    public SyncJobManager(AtomFeedService atomFeedService, DemographicsService demographicsService, ILogger<SyncJobManager> logger)
    {
        _atomFeedService = atomFeedService;
        _demographicsService = demographicsService;
        _logger = logger;
    }

    public async Task ExecuteFullSyncAsync(CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            if (_isFullSyncRunning)
            {
                _logger.LogWarning("Full sync is already running.");
                return;
            }

            _isFullSyncRunning = true;
        }

        try
        {
            var minDate = SqlDateTime.MinValue.Value;
            var maxDate = DateTime.Now;

            var newHires = _atomFeedService.GetFeedDataAsync("newhire", minDate, maxDate, cancellationToken);
            var updates = _atomFeedService.GetFeedDataAsync("empupdate", minDate, maxDate, cancellationToken);
            var terminations = _atomFeedService.GetFeedDataAsync("termination", minDate, maxDate, cancellationToken);

            await foreach (var record in MergeAsyncEnumerables(newHires, updates, terminations, cancellationToken))
            {
                _demographicsService.ProcessDemographicsAsync(record);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full sync.");
        }
        finally
        {
            lock (_syncLock)
            {
                _isFullSyncRunning = false;
            }
        }
    }

    public async Task ExecuteDeltaSyncAsync(CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            if (_isFullSyncRunning)
            {
                _logger.LogWarning("Cannot run delta sync while a full sync is running.");
                return;
            }
        }

        try
        {
            var maxDate = DateTime.Now;
            var minDate = maxDate.AddDays(-1);

            var newHires = _atomFeedService.GetFeedDataAsync("newhire", minDate, maxDate, cancellationToken);
            var updates = _atomFeedService.GetFeedDataAsync("empupdate", minDate, maxDate, cancellationToken);
            var terminations = _atomFeedService.GetFeedDataAsync("termination", minDate, maxDate, cancellationToken);

            await foreach (var record in MergeAsyncEnumerables(newHires, updates, terminations, cancellationToken))
            {
                _demographicsService.ProcessDemographicsAsync(record);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during delta sync.");
        }
    }

    private static async IAsyncEnumerable<AtomFeedRecord> MergeAsyncEnumerables(IAsyncEnumerable<AtomFeedRecord> first, IAsyncEnumerable<AtomFeedRecord> second,
        IAsyncEnumerable<AtomFeedRecord> third, [EnumeratorCancellation] CancellationToken cancellationToken)
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
    }
}
