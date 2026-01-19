using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// In-memory storage for per-user fake time settings.
/// Thread-safe singleton that stores user preferences.
/// </summary>
public sealed class UserFakeTimeStorage : IUserFakeTimeStorage
{
    private readonly ConcurrentDictionary<string, UserFakeTimeSettings> _userSettings = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<UserFakeTimeStorage>? _logger;

    public UserFakeTimeStorage(ILogger<UserFakeTimeStorage>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public UserFakeTimeSettings? GetSettings(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return _userSettings.TryGetValue(userId, out var settings) ? settings : null;
    }

    /// <inheritdoc />
    public void SetSettings(UserFakeTimeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.UserId);

        _userSettings[settings.UserId] = settings;

        _logger?.LogInformation(
            "User fake time settings SET for user {UserId}. FakeTime: {FakeTime:O}, AdvanceTime: {AdvanceTime}",
            settings.UserId,
            settings.GetCurrentFakeTime(),
            settings.Configuration.AdvanceTime);
    }

    /// <inheritdoc />
    public bool RemoveSettings(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var removed = _userSettings.TryRemove(userId, out var settings);

        if (removed)
        {
            _logger?.LogInformation(
                "User fake time settings REMOVED for user {UserId}. Was set to: {FakeTime:O}",
                userId,
                settings?.GetCurrentFakeTime());
        }

        return removed;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<UserFakeTimeSettings> GetAllSettings()
    {
        return _userSettings.Values.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public void ClearAll()
    {
        var count = _userSettings.Count;
        _userSettings.Clear();

        _logger?.LogWarning(
            "All user fake time settings CLEARED. {Count} user settings were removed.",
            count);
    }

    /// <inheritdoc />
    public int ActiveUserCount => _userSettings.Count;
}
