namespace Demoulas.ProfitSharing.Common.HostedServices;

/// <summary>
/// Configuration for the ProcessWatchdogService.
/// </summary>
public sealed class ProcessWatchdogConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether the process watchdog is enabled.
    /// Default: true
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout duration (in seconds) after which a missed heartbeat is detected.
    /// Default: 300 seconds (5 minutes)
    /// </summary>
    public int HeartbeatTimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Gets or sets the interval (in seconds) at which the watchdog checks for heartbeats.
    /// Default: 30 seconds
    /// </summary>
    public int CheckIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the number of missed heartbeats before a critical alert is issued.
    /// Default: 2
    /// </summary>
    public int AlertOnMissedHeartbeats { get; set; } = 2;
}
