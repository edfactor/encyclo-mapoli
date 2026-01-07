using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// A time provider that can return a configurable fake time for testing date-sensitive functionality.
/// This provider supports both frozen time (always returns the same instant) and advancing time
/// (starts at a fixed point but advances at real-time speed).
/// </summary>
/// <remarks>
/// SECURITY: This provider should only be used in non-production environments.
/// The service registration logic ensures this by checking the environment before registration.
/// </remarks>
public sealed class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _startUtcNow;
    private readonly DateTimeOffset _realStartTime;
    private readonly TimeZoneInfo _timeZone;
    private readonly bool _advanceTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class.
    /// </summary>
    /// <param name="configuration">The fake time configuration.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    public FakeTimeProvider(FakeTimeConfiguration configuration, ILogger<FakeTimeProvider>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _timeZone = configuration.GetTimeZone();
        _advanceTime = configuration.AdvanceTime;
        _realStartTime = DateTimeOffset.UtcNow;

        var parsedDateTime = configuration.GetParsedFixedDateTime();
        if (parsedDateTime.HasValue)
        {
            // Convert the local time to UTC using the configured time zone
            _startUtcNow = new DateTimeOffset(
                DateTime.SpecifyKind(parsedDateTime.Value, DateTimeKind.Unspecified),
                _timeZone.GetUtcOffset(parsedDateTime.Value));
        }
        else
        {
            // If no fixed time specified, start from now
            _startUtcNow = _realStartTime;
        }

        logger?.LogWarning(
            "FakeTimeProvider initialized. Fake UTC time: {FakeUtcTime:O}, Advance: {AdvanceTime}, TimeZone: {TimeZone}. " +
            "SECURITY WARNING: Fake time should only be used in non-production environments.",
            _startUtcNow,
            _advanceTime,
            _timeZone.Id);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class with a specific UTC time.
    /// Primarily used for unit testing.
    /// </summary>
    /// <param name="utcNow">The fixed UTC time to return.</param>
    /// <param name="advanceTime">Whether time should advance from the starting point.</param>
    public FakeTimeProvider(DateTimeOffset utcNow, bool advanceTime = false)
    {
        _startUtcNow = utcNow;
        _realStartTime = DateTimeOffset.UtcNow;
        _timeZone = TimeZoneInfo.Local;
        _advanceTime = advanceTime;
    }

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow()
    {
        if (_advanceTime)
        {
            // Calculate elapsed time since provider was created and add to fake start time
            var elapsed = DateTimeOffset.UtcNow - _realStartTime;
            return _startUtcNow + elapsed;
        }

        // Return frozen time
        return _startUtcNow;
    }

    /// <inheritdoc />
    public override TimeZoneInfo LocalTimeZone => _timeZone;

    /// <summary>
    /// Gets whether this provider is returning fake time.
    /// </summary>
    public static bool IsFakeTime => true;

    /// <summary>
    /// Gets the configured fake start time (UTC).
    /// </summary>
    public DateTimeOffset ConfiguredStartTimeUtc => _startUtcNow;

    /// <summary>
    /// Gets whether time is advancing from the start point.
    /// </summary>
    public bool IsAdvancingTime => _advanceTime;
}
