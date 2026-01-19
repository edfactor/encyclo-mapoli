using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

/// <summary>
/// Response containing a list of all users with fake time settings.
/// Used by administrators to view all active user time settings.
/// </summary>
[NoMemberDataExposed]
public sealed record AllUserFakeTimeResponse
{
    /// <summary>
    /// Total number of users with fake time settings.
    /// </summary>
    public int TotalUsers { get; init; }

    /// <summary>
    /// List of all user fake time settings.
    /// </summary>
    public required IReadOnlyList<UserFakeTimeStatusResponse> Users { get; init; }
}
