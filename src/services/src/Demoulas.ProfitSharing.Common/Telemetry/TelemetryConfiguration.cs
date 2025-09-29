namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Configuration options for telemetry features
/// </summary>
public class TelemetryConfiguration
{
    public const string SectionName = "Telemetry";

    /// <summary>
    /// Enable telemetry collection
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Enable sensitive field access counting (disabled by default in production)
    /// </summary>
    public bool EnableSensitiveFieldTracking { get; set; } = false;

    /// <summary>
    /// Threshold for large response detection (bytes)
    /// </summary>
    public long LargeResponseThresholdBytes { get; set; } = 5_000_000; // 5MB

    /// <summary>
    /// Include user role in metrics (low cardinality)
    /// </summary>
    public bool IncludeUserRole { get; set; } = true;

    /// <summary>
    /// Sample rate for detailed traces (0.0 to 1.0)
    /// </summary>
    public double DetailedTraceSampleRate { get; set; } = 0.1;
}
