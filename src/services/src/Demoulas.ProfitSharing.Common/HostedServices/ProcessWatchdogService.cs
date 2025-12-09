using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.HostedServices;

/// <summary>
/// A hosted service that monitors the health of the current process by watching for heartbeats.
/// </summary>
/// <remarks>
/// The ProcessWatchdogService acts as a watchdog for background services, ensuring that
/// the main processing loop is still executing. If the watchdog detects no heartbeat
/// for a configured duration, it logs critical errors and can trigger remedial action.
///
/// Services using this watchdog should:
/// 1. Inject IProcessWatchdog
/// 2. Call RecordHeartbeat() at regular intervals during processing
/// 3. Ensure heartbeats are frequent enough to stay within the timeout window
///
/// Configuration (via appsettings):
/// "ProcessWatchdog": {
///   "Enabled": true,
///   "HeartbeatTimeoutSeconds": 300,
///   "CheckIntervalSeconds": 30,
///   "AlertOnMissedHeartbeats": 2
/// }
/// </remarks>
internal sealed class ProcessWatchdogService : IProcessWatchdog, IHostedService
{
    private readonly ILogger<ProcessWatchdogService> _logger;
    private readonly ProcessWatchdogConfiguration _config;
    private Timer? _watchdogTimer;
    private DateTime _lastHeartbeat;
    private int _missedHeartbeats;

    public bool IsRunning { get; private set; }
    public DateTime? LastHeartbeat { get; private set; }
    public int MissedHeartbeats => _missedHeartbeats;

    public ProcessWatchdogService(ILogger<ProcessWatchdogService> logger, ProcessWatchdogConfiguration config)
    {
        _logger = logger;
        _config = config;
        _lastHeartbeat = DateTime.UtcNow;
        _missedHeartbeats = 0;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_config.Enabled)
        {
            _logger.LogInformation("Process watchdog is disabled");
            return Task.CompletedTask;
        }

        IsRunning = true;
        _logger.LogInformation(
            "Process watchdog started. Timeout: {TimeoutSeconds}s, Check interval: {CheckIntervalSeconds}s",
            _config.HeartbeatTimeoutSeconds,
            _config.CheckIntervalSeconds);

        _watchdogTimer = new Timer(
            CheckHeartbeat,
            null,
            TimeSpan.FromSeconds(_config.CheckIntervalSeconds),
            TimeSpan.FromSeconds(_config.CheckIntervalSeconds));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        if (_watchdogTimer is not null)
        {
            await _watchdogTimer.DisposeAsync().ConfigureAwait(false);
        }
        _logger.LogInformation("Process watchdog stopped");
    }

    public void RecordHeartbeat()
    {
        _lastHeartbeat = DateTime.UtcNow;
        LastHeartbeat = _lastHeartbeat;

        // Reset missed count when we get a heartbeat
        if (_missedHeartbeats > 0)
        {
            _logger.LogInformation(
                "Heartbeat received after {MissedHeartbeats} missed heartbeats. Service is healthy.",
                _missedHeartbeats);
            _missedHeartbeats = 0;
        }
    }

    private void CheckHeartbeat(object? state)
    {
        var timeSinceLastHeartbeat = DateTime.UtcNow - _lastHeartbeat;

        if (timeSinceLastHeartbeat.TotalSeconds > _config.HeartbeatTimeoutSeconds)
        {
            _missedHeartbeats++;

            _logger.LogWarning(
                "Watchdog: No heartbeat for {ElapsedSeconds}s (timeout: {TimeoutSeconds}s). Missed heartbeats: {MissedCount}",
                (int)timeSinceLastHeartbeat.TotalSeconds,
                _config.HeartbeatTimeoutSeconds,
                _missedHeartbeats);

            if (_missedHeartbeats >= _config.AlertOnMissedHeartbeats)
            {
                _logger.LogCritical(
                    "WATCHDOG ALERT: Process appears to be unresponsive. Missed {MissedCount} heartbeat checks. Last heartbeat: {LastHeartbeat}",
                    _missedHeartbeats,
                    _lastHeartbeat);
            }
        }
    }
}
