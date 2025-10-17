using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Caching.HostedServices;

/// <summary>
/// Hosted service that warms up the StateTaxCache on application startup.
/// This ensures all state tax rates are loaded from the database into the distributed cache
/// when the application starts, avoiding cold start delays on first request.
/// </summary>
public sealed class StateTaxCacheWarmerHostedService : BackgroundService
{
    private readonly StateTaxCache _cache;
    private readonly ILogger<StateTaxCacheWarmerHostedService> _logger;

    public StateTaxCacheWarmerHostedService(
        StateTaxCache cache,
        ILogger<StateTaxCacheWarmerHostedService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Warming up StateTaxCache on startup");

            // Trigger cache load from database on startup
            var allData = await _cache.GetAllAsync(stoppingToken);

            _logger.LogInformation("StateTaxCache warmed up successfully with {Count} state tax rates", allData.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up StateTaxCache on startup");
            throw; // Fail fast on startup if cache warmup fails
        }
    }
}
