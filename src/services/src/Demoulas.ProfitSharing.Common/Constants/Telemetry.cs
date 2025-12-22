namespace Demoulas.ProfitSharing.Common.Constants;

/// <summary>
/// Centralized telemetry constants used across the application for consistent tracking and correlation.
/// </summary>
public static class Telemetry
{
    /// <summary>
    /// HttpContext.Items key for storing the session ID (20-character GUID).
    /// Session ID is created by middleware on the first request and stored in both Items and response cookies.
    /// </summary>
    public const string SessionIdKey = "ps-session-id";
}
