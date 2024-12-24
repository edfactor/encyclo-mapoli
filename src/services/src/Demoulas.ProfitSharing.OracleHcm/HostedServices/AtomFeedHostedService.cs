using Demoulas.ProfitSharing.OracleHcm.Atom;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

public class AtomFeedHostedService : BackgroundService
{
    private readonly SyncJobManager _syncJobManager;
    private readonly ILogger<AtomFeedHostedService> _logger;

    public AtomFeedHostedService(SyncJobManager syncJobManager, ILogger<AtomFeedHostedService> logger)
    {
        _syncJobManager = syncJobManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Atom Feed Hosted Service.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _syncJobManager.ExecuteFullSyncAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running delta sync in Atom Feed Hosted Service.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        _logger.LogInformation("Stopping Atom Feed Hosted Service.");
    }
}
