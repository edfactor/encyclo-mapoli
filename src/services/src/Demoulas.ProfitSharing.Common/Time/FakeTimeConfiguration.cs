using System.Globalization;

namespace Demoulas.ProfitSharing.Common.Time;

/// <summary>
/// Configuration for the fake time provider, used for testing date-sensitive functionality.
/// SECURITY: Fake time is disabled by default and MUST NOT be enabled in Production environments.
/// </summary>
public sealed class FakeTimeConfiguration
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "FakeTime";

    /// <summary>
    /// Gets or sets whether fake time is enabled.
    /// Default is false. When false, the system uses real time.
    /// SECURITY: Must remain false in Production environments.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the fixed date/time to use when fake time is enabled.
    /// Format: ISO 8601 (e.g., "2025-12-15T10:00:00").
    /// If null or empty when Enabled is true, uses the current real time.
    /// </summary>
    public string? FixedDateTime { get; set; }

    /// <summary>
    /// Gets or sets the time zone for local time calculations.
    /// Uses Windows time zone IDs (e.g., "Eastern Standard Time") or IANA IDs (e.g., "America/New_York").
    /// Default is null, which uses the system's local time zone.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets whether to advance time automatically at real-time speed from the fixed starting point.
    /// When true, time advances normally from <see cref="FixedDateTime"/>.
    /// When false, time remains frozen at <see cref="FixedDateTime"/>.
    /// Default is false (frozen time).
    /// </summary>
    public bool AdvanceTime { get; set; }

    /// <summary>
    /// Validates the configuration and returns any validation errors.
    /// </summary>
    /// <returns>A list of validation error messages, empty if valid.</returns>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (Enabled && !string.IsNullOrWhiteSpace(FixedDateTime) &&
            !DateTime.TryParse(FixedDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            errors.Add($"FakeTime.FixedDateTime '{FixedDateTime}' is not a valid date/time format. Use ISO 8601 format (e.g., '2025-12-15T10:00:00').");
        }

        if (!string.IsNullOrWhiteSpace(TimeZone))
        {
            try
            {
                _ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                errors.Add($"FakeTime.TimeZone '{TimeZone}' is not a valid time zone identifier.");
            }
        }

        return errors;
    }

    /// <summary>
    /// Gets the parsed fixed date/time, or null if not configured or invalid.
    /// </summary>
    public DateTime? GetParsedFixedDateTime()
    {
        if (string.IsNullOrWhiteSpace(FixedDateTime))
        {
            return null;
        }

        return DateTime.TryParse(FixedDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null;
    }

    /// <summary>
    /// Gets the configured time zone, or the local system time zone if not specified.
    /// </summary>
    public TimeZoneInfo GetTimeZone()
    {
        if (string.IsNullOrWhiteSpace(TimeZone))
        {
            return TimeZoneInfo.Local;
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Local;
        }
    }
}
