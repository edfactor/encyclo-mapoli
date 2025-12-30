namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Represents the health status of a monitored process.
/// </summary>
public record ProcessHealth
{
    /// <summary>
    /// Gets a value indicating whether the process is currently healthy.
    /// </summary>
    public bool IsHealthy { get; init; }

    /// <summary>
    /// Gets the current state of the process (Healthy, Unresponsive, Critical, etc.).
    /// </summary>
    public string State { get; init; } = "Unknown";

    /// <summary>
    /// Gets the last heartbeat timestamp from the monitored process.
    /// </summary>
    public DateTime? LastHeartbeat { get; init; }

    /// <summary>
    /// Gets the time elapsed since the last heartbeat.
    /// </summary>
    public TimeSpan TimeSinceLastHeartbeat { get; init; }

    /// <summary>
    /// Gets the number of missed heartbeats (exceeded timeout without heartbeat).
    /// </summary>
    public int MissedHeartbeats { get; init; }

    /// <summary>
    /// Gets a detailed status message describing the current health state.
    /// </summary>
    public string StatusMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of errors that have been recorded.
    /// </summary>
    public int ErrorCount { get; init; }

    /// <summary>
    /// Gets the last error message that was recorded, if any.
    /// </summary>
    public string? LastErrorMessage { get; init; }

    /// <summary>
    /// Gets the timestamp of the last recorded error.
    /// </summary>
    public DateTime? LastErrorTime { get; init; }

    /// <summary>
    /// Gets the number of successful work cycles completed.
    /// </summary>
    public int SuccessfulCycles { get; init; }

    /// <summary>
    /// Gets the timestamp of the last successful work cycle.
    /// </summary>
    public DateTime? LastSuccessfulCycle { get; init; }
}
