using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// Scoped service that resolves time based on the current user's fake time settings.
/// Falls back to real system time if the user has no settings or in production.
/// </summary>
public sealed class UserTimeService : IUserTimeService
{
    private readonly IUserFakeTimeStorage _storage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeProvider _systemTimeProvider;
    private UserFakeTimeSettings? _cachedSettings;
    private bool _settingsResolved;

    public UserTimeService(
        IUserFakeTimeStorage storage,
        IHttpContextAccessor httpContextAccessor,
        TimeProvider systemTimeProvider)
    {
        _storage = storage;
        _httpContextAccessor = httpContextAccessor;
        _systemTimeProvider = systemTimeProvider;
    }

    /// <inheritdoc />
    public string? CurrentUserId => GetUserId();

    /// <inheritdoc />
    public UserFakeTimeSettings? CurrentUserSettings => GetUserSettings();

    /// <inheritdoc />
    public bool IsUsingFakeTime => GetUserSettings() is not null;

    /// <inheritdoc />
    public TimeZoneInfo LocalTimeZone
    {
        get
        {
            var settings = GetUserSettings();
            return settings?.GetTimeZone() ?? _systemTimeProvider.LocalTimeZone;
        }
    }

    /// <inheritdoc />
    public DateTimeOffset GetUtcNow()
    {
        var settings = GetUserSettings();
        if (settings is not null)
        {
            return settings.GetCurrentFakeTime().ToUniversalTime();
        }

        return _systemTimeProvider.GetUtcNow();
    }

    /// <inheritdoc />
    public DateTimeOffset GetLocalNow()
    {
        var settings = GetUserSettings();
        if (settings is not null)
        {
            var fakeTime = settings.GetCurrentFakeTime();
            var timeZone = settings.GetTimeZone();
            return TimeZoneInfo.ConvertTime(fakeTime, timeZone);
        }

        return _systemTimeProvider.GetLocalNow();
    }

    /// <inheritdoc />
    public DateOnly GetLocalDateOnly()
    {
        return DateOnly.FromDateTime(GetLocalNow().DateTime);
    }

    /// <inheritdoc />
    public int GetLocalYear()
    {
        return GetLocalNow().Year;
    }

    /// <inheritdoc />
    public short GetLocalYearAsShort()
    {
        return (short)GetLocalNow().Year;
    }

    /// <inheritdoc />
    public int GetLocalMonth()
    {
        return GetLocalNow().Month;
    }

    private string? GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // Use "sub" claim (standard OIDC) or fall back to Identity.Name
        return httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.Identity?.Name;
    }

    private UserFakeTimeSettings? GetUserSettings()
    {
        // Cache the settings for the lifetime of the request (scoped service)
        if (_settingsResolved)
        {
            return _cachedSettings;
        }

        _settingsResolved = true;

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _cachedSettings = null;
            return null;
        }

        _cachedSettings = _storage.GetSettings(userId);
        return _cachedSettings;
    }
}
