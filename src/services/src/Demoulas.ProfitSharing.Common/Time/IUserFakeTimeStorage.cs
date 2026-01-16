namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// Storage interface for per-user fake time settings.
/// This is a singleton service that stores user preferences in memory.
/// </summary>
public interface IUserFakeTimeStorage
{
    /// <summary>
    /// Gets the fake time settings for a specific user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>The user's settings, or null if not set.</returns>
    UserFakeTimeSettings? GetSettings(string userId);

    /// <summary>
    /// Sets or updates the fake time settings for a specific user.
    /// </summary>
    /// <param name="settings">The settings to store.</param>
    void SetSettings(UserFakeTimeSettings settings);

    /// <summary>
    /// Removes the fake time settings for a specific user (returns to real time).
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>True if settings were removed; false if no settings existed.</returns>
    bool RemoveSettings(string userId);

    /// <summary>
    /// Gets all active user fake time settings (for admin monitoring).
    /// </summary>
    /// <returns>All active settings.</returns>
    IReadOnlyCollection<UserFakeTimeSettings> GetAllSettings();

    /// <summary>
    /// Clears all user fake time settings.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Gets the count of users with active fake time settings.
    /// </summary>
    int ActiveUserCount { get; }
}
