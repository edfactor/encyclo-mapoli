namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Interface for a process watchdog service that monitors the health and continuity of a running process.
/// </summary>
/// <remarks>
/// The watchdog service is responsible for detecting if the process has stopped unexpectedly
/// and taking corrective action (such as logging, alerting, or restarting).
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
    /// Records a heartbeat from the monitored process to indicate it is still alive.
    /// </summary>
    /// <remarks>
    /// This method should be called periodically by the monitored process (typically at the end of each work cycle).
    /// </remarks>
    void RecordHeartbeat();

    /// <summary>
    /// Gets the number of missed heartbeats (exceeded timeout without heartbeat).
    /// </summary>
    int MissedHeartbeats { get; }
}
