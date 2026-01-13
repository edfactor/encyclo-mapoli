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
    private readonly string _serviceName;
    private readonly string _environmentName;
    private readonly string _machineName;
    private readonly int _processId;
    private Timer? _watchdogTimer;
    private DateTime _lastHeartbeat;
    private int _missedHeartbeats;
    private bool _alertRaised;
    private int _errorCount;
    private string? _lastErrorMessage;
    private DateTime? _lastErrorTime;
    private int _successfulCycles;
    private DateTime? _lastSuccessfulCycle;

    public bool IsRunning { get; private set; }
    public DateTime? LastHeartbeat { get; private set; }
    public int MissedHeartbeats => _missedHeartbeats;

    public ProcessWatchdogService(
        ILogger<ProcessWatchdogService> logger,
        ProcessWatchdogConfiguration config,
        IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _config = config;
        _serviceName = hostEnvironment.ApplicationName;
        _environmentName = hostEnvironment.EnvironmentName;
        _machineName = Environment.MachineName;
        _processId = Environment.ProcessId;
        _lastHeartbeat = DateTime.UtcNow;
        _missedHeartbeats = 0;
        _alertRaised = false;
        _errorCount = 0;
        _lastErrorMessage = null;
        _lastErrorTime = null;
        _successfulCycles = 0;
        _lastSuccessfulCycle = null;
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
            "Process watchdog started for {ServiceName} ({EnvironmentName}) on {MachineName} PID {ProcessId}. Timeout: {TimeoutSeconds}s, Check interval: {CheckIntervalSeconds}s",
            _serviceName,
            _environmentName,
            _machineName,
            _processId,
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
            _alertRaised = false;
        }
    }

    public void RecordSuccessfulCycle()
    {
        _successfulCycles++;
        _lastSuccessfulCycle = DateTime.UtcNow;
        _logger.LogDebug("Successful work cycle recorded. Total cycles: {CycleCount}", _successfulCycles);
    }

    public void RecordError(string errorMessage)
    {
        _errorCount++;
        _lastErrorMessage = errorMessage;
        _lastErrorTime = DateTime.UtcNow;
        _logger.LogWarning("Error recorded: {ErrorMessage} (Error count: {ErrorCount})", errorMessage, _errorCount);
    }

    public ProcessHealth GetHealth()
    {
        var timeSinceLastHeartbeat = DateTime.UtcNow - _lastHeartbeat;
        bool isHealthy = timeSinceLastHeartbeat.TotalSeconds <= _config.HeartbeatTimeoutSeconds
                         && _missedHeartbeats == 0
                         && _errorCount == 0;
        string state = DetermineState(timeSinceLastHeartbeat);
        string statusMessage = GenerateStatusMessage(timeSinceLastHeartbeat, state);

        return new ProcessHealth
        {
            IsHealthy = isHealthy,
            State = state,
            LastHeartbeat = LastHeartbeat,
            TimeSinceLastHeartbeat = timeSinceLastHeartbeat,
            MissedHeartbeats = _missedHeartbeats,
            StatusMessage = statusMessage,
            ErrorCount = _errorCount,
            LastErrorMessage = _lastErrorMessage,
            LastErrorTime = _lastErrorTime,
            SuccessfulCycles = _successfulCycles,
            LastSuccessfulCycle = _lastSuccessfulCycle
        };
    }

    private string DetermineState(TimeSpan timeSinceLastHeartbeat)
    {
        if (!IsRunning)
        {
            return "Stopped";
        }

        if (_errorCount > 0)
        {
            return "Error";
        }

        if (timeSinceLastHeartbeat.TotalSeconds > _config.HeartbeatTimeoutSeconds)
        {
            if (_missedHeartbeats >= _config.AlertOnMissedHeartbeats)
            {
                return "Critical";
            }

            return "Unresponsive";
        }

        return "Healthy";
    }

    private string GenerateStatusMessage(TimeSpan timeSinceLastHeartbeat, string state)
    {
        return state switch
        {
            "Healthy" => "Process is running normally with regular heartbeats.",
            "Unresponsive" => $"Process has not responded for {(int)timeSinceLastHeartbeat.TotalSeconds}s. Missed {_missedHeartbeats} heartbeat(s).",
            "Critical" => $"CRITICAL: Process appears unresponsive for {(int)timeSinceLastHeartbeat.TotalSeconds}s. Missed {_missedHeartbeats} heartbeat checks.",
            "Error" => $"Process has recorded {_errorCount} error(s). Last error: {_lastErrorMessage}",
            "Stopped" => "Watchdog is not running.",
            _ => "Status unknown."
        };
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

            if (_missedHeartbeats >= _config.AlertOnMissedHeartbeats && !_alertRaised)
            {
                _alertRaised = true;

                int elapsedSeconds = (int)timeSinceLastHeartbeat.TotalSeconds;
                _logger.LogCritical(
                    "WATCHDOG ALERT: {ServiceName} ({EnvironmentName}) on {MachineName} PID {ProcessId} appears to be unresponsive. Missed {MissedCount} heartbeat checks (threshold: {Threshold}). Last heartbeat (UTC): {LastHeartbeatUtc:O} ({ElapsedSeconds}s ago). Timeout: {TimeoutSeconds}s.",
                    _serviceName,
                    _environmentName,
                    _machineName,
                    _processId,
                    _missedHeartbeats,
                    _config.AlertOnMissedHeartbeats,
                    _lastHeartbeat,
                    elapsedSeconds,
                    _config.HeartbeatTimeoutSeconds);
            }
        }
    }
}
