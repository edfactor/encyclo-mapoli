using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

/// <summary>
/// Request DTO for setting fake time configuration.
/// SECURITY: Only allowed in non-Production environments.
/// </summary>
public sealed class SetFakeTimeRequest
{
    /// <summary>
    /// Gets or sets whether to enable fake time.
    /// Set to false to disable and return to real system time.
    /// </summary>
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the fixed date/time to use when fake time is enabled.
    /// Format: ISO 8601 (e.g., "2025-12-15T10:00:00").
    /// Required when Enabled is true.
    /// </summary>
    public string? FixedDateTime { get; set; }

    /// <summary>
    /// Gets or sets the time zone for local time calculations.
    /// Uses Windows time zone IDs (e.g., "Eastern Standard Time") or IANA IDs (e.g., "America/New_York").
    /// If null, uses the system's local time zone.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets whether to advance time automatically at real-time speed from the fixed starting point.
    /// When true, time advances normally from <see cref="FixedDateTime"/>.
    /// When false, time remains frozen at <see cref="FixedDateTime"/>.
    /// Default is false (frozen time).
    /// </summary>
    public bool AdvanceTime { get; set; }
}
