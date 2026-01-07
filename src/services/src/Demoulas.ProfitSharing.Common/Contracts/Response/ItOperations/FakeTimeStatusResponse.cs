using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

/// <summary>
/// Response DTO for fake time status endpoint.
/// </summary>
[NoMemberDataExposed]
public sealed class FakeTimeStatusResponse
{
    /// <summary>
    /// Gets or sets whether fake time is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether fake time is allowed in the current environment.
    /// False in Production environments.
    /// </summary>
    public bool IsAllowed { get; set; }

    /// <summary>
    /// Gets or sets the current fake date/time when active (ISO 8601 format).
    /// Null when fake time is not active.
    /// </summary>
    public string? CurrentFakeDateTime { get; set; }

    /// <summary>
    /// Gets or sets the configured fixed date/time (ISO 8601 format).
    /// </summary>
    public string? ConfiguredDateTime { get; set; }

    /// <summary>
    /// Gets or sets the configured time zone identifier.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets whether time advances automatically from the configured starting point.
    /// </summary>
    public bool AdvanceTime { get; set; }

    /// <summary>
    /// Gets or sets the current environment name (Development, QA, UAT, Production).
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the system's real current date/time for reference (ISO 8601 format).
    /// </summary>
    public string RealDateTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an informational message about the fake time status.
    /// </summary>
    public string? Message { get; set; }
}
