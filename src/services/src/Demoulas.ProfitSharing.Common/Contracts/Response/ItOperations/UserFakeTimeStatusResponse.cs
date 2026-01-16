using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

/// <summary>
/// Response containing the current user's fake time status.
/// This is user-specific, not the global server fake time status.
/// </summary>
[NoMemberDataExposed]
public sealed record UserFakeTimeStatusResponse
{
    /// <summary>
    /// Whether the current user has fake time active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Whether per-user fake time is allowed in this environment.
    /// Always false in Production.
    /// </summary>
    public bool IsAllowed { get; init; }

    /// <summary>
    /// The current fake date/time for this user, if active.
    /// </summary>
    public string? CurrentFakeDateTime { get; init; }

    /// <summary>
    /// The configured fixed date/time string for this user (from their settings).
    /// </summary>
    public string? ConfiguredDateTime { get; init; }

    /// <summary>
    /// The timezone being used for this user's fake time.
    /// </summary>
    public string? TimeZone { get; init; }

    /// <summary>
    /// Whether time advances from the configured start for this user.
    /// </summary>
    public bool AdvanceTime { get; init; }

    /// <summary>
    /// The current real system date/time for reference.
    /// </summary>
    public required string RealDateTime { get; init; }

    /// <summary>
    /// The user ID this setting applies to.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Human-readable message about the current state.
    /// </summary>
    public string? Message { get; init; }
}
