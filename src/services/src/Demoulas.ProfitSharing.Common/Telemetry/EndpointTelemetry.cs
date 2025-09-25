using System.Diagnostics;
using System.Diagnostics.Metrics;
using Demoulas.ProfitSharing.Common.Metrics;

namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Extends the existing GlobalMeter with endpoint-specific telemetry
/// </summary>
public static class EndpointTelemetry
{
    // Activity source for distributed tracing
    public static readonly ActivitySource ActivitySource = new(GlobalMeter.Name, "1.0.0");

    // Use the existing GlobalMeter - avoid duplication
    public static Meter Meter => GlobalMeter.Meter;

    // Endpoint-specific metrics (extending the existing pattern)
    public static Counter<long> SensitiveFieldAccessTotal { get; private set; } = null!;
    public static Counter<long> LargeResponsesTotal { get; private set; } = null!;
    public static Counter<long> EndpointErrorsTotal { get; private set; } = null!;
    public static Histogram<long> ResponseSizeBytes { get; private set; } = null!;

    private static bool _initialized;
    private static readonly object _initLock = new();

    /// <summary>
    /// Initialize endpoint-specific metrics. Call once at startup.
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;

        lock (_initLock)
        {
            if (_initialized) return;

            SensitiveFieldAccessTotal = Meter.CreateCounter<long>(
                "ps_sensitive_field_access_total",
                "Total access count for sensitive fields");

            LargeResponsesTotal = Meter.CreateCounter<long>(
                "ps_large_responses_total",
                "Total count of responses exceeding size threshold");

            EndpointErrorsTotal = Meter.CreateCounter<long>(
                "ps_endpoint_errors_total",
                "Total count of endpoint errors by type");

            ResponseSizeBytes = Meter.CreateHistogram<long>(
                "ps_response_size_bytes",
                "Size of HTTP responses in bytes");

            _initialized = true;
        }
    }
}
