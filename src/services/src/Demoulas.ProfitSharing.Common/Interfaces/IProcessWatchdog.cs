namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Interface for a process watchdog service that monitors the health and continuity of a running process.
/// </summary>
/// <remarks>
/// The watchdog service is responsible for detecting if the process has stopped unexpectedly
/// and taking corrective action (such as logging, alerting, or restarting).
/// 
/// It tracks:
/// - Heartbeats: Regular signals that the process is alive
/// - Work cycles: Successful completion of work units
/// - Errors: Exceptions and failures during processing
/// - Health state: Overall process health status
/// </remarks>
public interface IProcessWatchdog
{
    /// <summary>
    /// Gets a value indicating whether the watchdog is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets the last heartbeat timestamp from the monitored process.
    /// </summary>
    DateTime? LastHeartbeat { get; }

    /// <summary>
    /// Gets the number of missed heartbeats (exceeded timeout without heartbeat).
    /// </summary>
    int MissedHeartbeats { get; }

    /// <summary>
    /// Records a heartbeat from the monitored process to indicate it is still alive.
    /// </summary>
    /// <remarks>
    /// This method should be called periodically by the monitored process (typically at the end of each work cycle).
    /// </remarks>
    void RecordHeartbeat();

    /// <summary>
    /// Records a successful work cycle completion.
    /// </summary>
    /// <remarks>
    /// This should be called when a unit of work (e.g., batch processing, data sync) completes successfully.
    /// This is distinct from RecordHeartbeat in that it specifically tracks successful completion.
    /// </remarks>
    void RecordSuccessfulCycle();

    /// <summary>
    /// Records an error that occurred during processing.
    /// </summary>
    /// <param name="errorMessage">A description of the error that occurred.</param>
    /// <remarks>
    /// Call this when an exception or error condition is detected.
    /// The watchdog will track error counts and the most recent error message.
    /// </remarks>
    void RecordError(string errorMessage);

    /// <summary>
    /// Gets the current health status of the monitored process.
    /// </summary>
    /// <returns>A ProcessHealth record containing detailed health information.</returns>
    ProcessHealth GetHealth();
}
