namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// Service interface for resolving time based on the current user's settings.
/// This is a scoped service that checks if the current user has fake time enabled.
/// </summary>
public interface IUserTimeService
{
    /// <summary>
    /// Gets the current UTC time for the current user.
    /// Returns fake time if the user has it enabled, otherwise returns real system time.
    /// </summary>
    /// <returns>The current UTC time (real or fake based on user settings).</returns>
    DateTimeOffset GetUtcNow();

    /// <summary>
    /// Gets the current local time for the current user.
    /// Returns fake time if the user has it enabled, otherwise returns real system time.
    /// </summary>
    /// <returns>The current local time (real or fake based on user settings).</returns>
    DateTimeOffset GetLocalNow();

    /// <summary>
    /// Gets the current local date for the current user.
    /// </summary>
    /// <returns>The current local date.</returns>
    DateOnly GetLocalDateOnly();

    /// <summary>
    /// Gets the current local year for the current user.
    /// </summary>
    /// <returns>The current local year.</returns>
    int GetLocalYear();

    /// <summary>
    /// Gets the current local year as a short for the current user.
    /// </summary>
    /// <returns>The current local year as Int16.</returns>
    short GetLocalYearAsShort();

    /// <summary>
    /// Gets the current local month for the current user.
    /// </summary>
    /// <returns>The current local month (1-12).</returns>
    int GetLocalMonth();

    /// <summary>
    /// Gets whether the current user has fake time enabled.
    /// </summary>
    bool IsUsingFakeTime { get; }

    /// <summary>
    /// Gets the current user's fake time settings, or null if using real time.
    /// </summary>
    UserFakeTimeSettings? CurrentUserSettings { get; }

    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    string? CurrentUserId { get; }

    /// <summary>
    /// Gets the time zone being used (user's fake time zone or system default).
    /// </summary>
    TimeZoneInfo LocalTimeZone { get; }
}
