using System.Runtime.CompilerServices;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public sealed class SyncJobService
{
    private readonly AtomFeedService _atomFeedService;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly ILogger<SyncJobService> _logger;
    private static readonly object _syncLock = new object();
    private bool _isDeltaSyncRunning;

    public SyncJobService(AtomFeedService atomFeedService, IDemographicsServiceInternal demographicsService, ILogger<SyncJobService> logger)
    {
        _atomFeedService = atomFeedService;
        _demographicsService = demographicsService;
        _logger = logger;
    }

    public async Task ExecuteDeltaSyncAsync(CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            if (_isDeltaSyncRunning)
            {
                _logger.LogWarning("Full sync is already running.");
                return;
            }

            _isDeltaSyncRunning = true;
        }

        try
        {
            var maxDate = DateTime.Now;
            var minDate = maxDate.AddYears(-1);

            var newHires = _atomFeedService.GetFeedDataAsync("newhire", minDate, maxDate, cancellationToken);
            var updates = _atomFeedService.GetFeedDataAsync("empupdate", minDate, maxDate, cancellationToken);
            var terminations = _atomFeedService.GetFeedDataAsync("termination", minDate, maxDate, cancellationToken);

            await foreach (var record in MergeAsyncEnumerables(newHires, updates, terminations, cancellationToken))
            {
                _demographicsService.ProcessDemographics(record);
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
                _isDeltaSyncRunning = false;
            }
        }
    }


    private static async IAsyncEnumerable<Context> MergeAsyncEnumerables(IAsyncEnumerable<Context> first, IAsyncEnumerable<Context> second,
        IAsyncEnumerable<Context> third, [EnumeratorCancellation] CancellationToken cancellationToken)
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
