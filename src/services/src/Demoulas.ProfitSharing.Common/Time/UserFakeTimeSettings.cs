namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// Represents fake time settings for a specific user.
/// </summary>
public sealed class UserFakeTimeSettings
{
    /// <summary>
    /// Gets or sets the user ID (from claims).
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets or sets when these settings were created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the fake time configuration for this user.
    /// </summary>
    public required FakeTimeConfiguration Configuration { get; init; }

    /// <summary>
    /// Gets or sets the real time when fake time was activated (for advancing time calculations).
    /// </summary>
    public DateTimeOffset RealTimeAtActivation { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the current fake time based on the configuration.
    /// </summary>
    /// <returns>The fake DateTimeOffset.</returns>
    public DateTimeOffset GetCurrentFakeTime()
    {
        var parsedTime = Configuration.GetParsedFixedDateTime();
        if (!parsedTime.HasValue)
        {
            return DateTimeOffset.UtcNow;
        }

        var timeZone = Configuration.GetTimeZone();
        var fakeStartTime = new DateTimeOffset(
            DateTime.SpecifyKind(parsedTime.Value, DateTimeKind.Unspecified),
            timeZone.GetUtcOffset(parsedTime.Value));

        if (Configuration.AdvanceTime)
        {
            // Calculate elapsed real time since activation and add to fake start time
            var elapsed = DateTimeOffset.UtcNow - RealTimeAtActivation;
            return fakeStartTime + elapsed;
        }

        // Return frozen time
        return fakeStartTime;
    }

    /// <summary>
    /// Gets the configured time zone.
    /// </summary>
    public TimeZoneInfo GetTimeZone() => Configuration.GetTimeZone();
}
