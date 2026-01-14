using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;

public class MemoryMonitoringService : BackgroundService
{
    private readonly ILogger<MemoryMonitoringService> _logger;
    private const long MemoryLimitBytes = 2L * 1024 * 1024 * 1024; // 2GB

    public MemoryMonitoringService(ILogger<MemoryMonitoringService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            long memoryUsed = Process.GetCurrentProcess().PrivateMemorySize64;

            if (memoryUsed > MemoryLimitBytes)
            {
                _logger.LogCritical("Memory limit exceeded: {MemoryUsed} MB. Shutting down...", (memoryUsed / (1024 * 1024)));

                // Initiate shutdown
                Environment.Exit(1);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken).ConfigureAwait(false); // Check every 10 seconds
        }
    }
}
