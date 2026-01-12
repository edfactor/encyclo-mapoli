using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.SystemInfo;

/// <summary>
/// Response containing the current server time information.
/// Used for synchronizing client applications with the server's time perception.
/// </summary>
[NoMemberDataExposed]
public sealed class CurrentTimeResponse
{
    /// <summary>
    /// Gets or sets the current UTC time.
    /// </summary>
    public DateTimeOffset UtcNow { get; set; }

    /// <summary>
    /// Gets or sets the current local time (in the server's configured time zone).
    /// </summary>
    public DateTimeOffset LocalNow { get; set; }

    /// <summary>
    /// Gets or sets the server's configured time zone ID.
    /// </summary>
    public string TimeZoneId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the server's time zone.
    /// </summary>
    public string TimeZoneDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the server is using fake/simulated time.
    /// This is always false in Production environments.
    /// </summary>
    public bool IsFakeTime { get; set; }

    /// <summary>
    /// Gets or sets the current fiscal year based on the server's local time.
    /// </summary>
    public int CurrentYear { get; set; }

    /// <summary>
    /// Gets or sets the current month based on the server's local time (1-12).
    /// </summary>
    public int CurrentMonth { get; set; }

    /// <summary>
    /// Gets or sets the current local date as a string (yyyy-MM-dd format).
    /// </summary>
    public string CurrentDate { get; set; } = string.Empty;
}
