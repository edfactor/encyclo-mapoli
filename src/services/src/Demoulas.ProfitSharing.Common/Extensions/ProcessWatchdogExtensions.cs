using Demoulas.ProfitSharing.Common.HostedServices;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Common.Extensions;

/// <summary>
/// Extension methods for registering the process watchdog service.
/// </summary>
public static class ProcessWatchdogExtensions
{
    /// <summary>
    /// Adds the process watchdog service to the host.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to configure.</param>
    /// <remarks>
    /// Registers the ProcessWatchdogService as both a singleton and hosted service.
    /// Configuration is read from the "ProcessWatchdog" section in appsettings.
    ///
    /// Example appsettings configuration:
    /// <code>
    /// {
    ///   "ProcessWatchdog": {
    ///     "Enabled": true,
    ///     "HeartbeatTimeoutSeconds": 300,
    ///     "CheckIntervalSeconds": 30,
    ///     "AlertOnMissedHeartbeats": 2
    ///   }
    /// }
    /// </code>
    ///
    /// Usage in a background service:
    /// <code>
    /// public class MyBackgroundService : IHostedService
    /// {
    ///     private readonly IProcessWatchdog _watchdog;
    ///
    ///     public MyBackgroundService(IProcessWatchdog watchdog) => _watchdog = watchdog;
    ///
    ///     public async Task ExecuteAsync(CancellationToken ct)
    ///     {
    ///         while (!ct.IsCancellationRequested)
    ///         {
    ///             // Do work...
    ///             _watchdog.RecordHeartbeat();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/>.</returns>
    public static IHostApplicationBuilder AddProcessWatchdog(this IHostApplicationBuilder builder)
    {
        var config = new ProcessWatchdogConfiguration();
        builder.Configuration.Bind("ProcessWatchdog", config);

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IProcessWatchdog, ProcessWatchdogService>();
        builder.Services.AddHostedService(sp => (ProcessWatchdogService)sp.GetRequiredService<IProcessWatchdog>());

        return builder;
    }

    /// <summary>
    /// Adds the process watchdog service with custom configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to configure.</param>
    /// <param name="configure">Action to configure the watchdog settings.</param>
    /// <remarks>
    /// Use this overload when you need to configure the watchdog programmatically
    /// instead of via appsettings.
    /// </remarks>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/>.</returns>
    public static IHostApplicationBuilder AddProcessWatchdog(this IHostApplicationBuilder builder,
        Action<ProcessWatchdogConfiguration> configure)
    {
        var config = new ProcessWatchdogConfiguration();
        builder.Configuration.Bind("ProcessWatchdog", config);
        configure(config);

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IProcessWatchdog, ProcessWatchdogService>();
        builder.Services.AddHostedService(sp => (ProcessWatchdogService)sp.GetRequiredService<IProcessWatchdog>());

        return builder;
    }
}
