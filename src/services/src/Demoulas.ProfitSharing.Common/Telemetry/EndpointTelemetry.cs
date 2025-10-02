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

    // Additional comprehensive metrics for endpoint monitoring
    public static Histogram<long> RequestSizeBytes { get; private set; } = null!;
    public static Histogram<double> EndpointDurationMs { get; private set; } = null!;
    public static Counter<long> UserActivityTotal { get; private set; } = null!;
    public static Counter<long> BusinessOperationsTotal { get; private set; } = null!;
    public static Histogram<long> RecordCountsProcessed { get; private set; } = null!;
    public static Counter<long> ValidationFailuresTotal { get; private set; } = null!;

    // Enhanced telemetry metrics for database and cache performance
    public static Histogram<double> DatabaseQueryDurationMs { get; private set; } = null!;
    public static Histogram<double> BusinessLogicDurationMs { get; private set; } = null!;
    public static Counter<long> CacheHitsTotal { get; private set; } = null!;
    public static Counter<long> CacheMissesTotal { get; private set; } = null!;
    public static Counter<long> DatabaseOperationsTotal { get; private set; } = null!;

    private static bool _initialized;
    private static readonly object _initLock = new();

    /// <summary>
    /// Initialize endpoint-specific metrics. Call once at startup.
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) { return; }

        lock (_initLock)
        {
            if (_initialized) { return; }

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

            // Additional comprehensive metrics for endpoint monitoring
            RequestSizeBytes = Meter.CreateHistogram<long>(
                "ps_request_size_bytes",
                "Size of HTTP requests in bytes");

            EndpointDurationMs = Meter.CreateHistogram<double>(
                "ps_endpoint_duration_ms",
                "Endpoint execution duration in milliseconds");

            UserActivityTotal = Meter.CreateCounter<long>(
                "ps_user_activity_total",
                "User activity count by role and endpoint");

            BusinessOperationsTotal = Meter.CreateCounter<long>(
                "ps_business_operations_total",
                "Business operations count by type");

            RecordCountsProcessed = Meter.CreateHistogram<long>(
                "ps_record_counts",
                "Number of records returned or processed");

            ValidationFailuresTotal = Meter.CreateCounter<long>(
                "ps_validation_failures_total",
                "Request validation failures by type");

            // Enhanced telemetry metrics for performance analysis
            DatabaseQueryDurationMs = Meter.CreateHistogram<double>(
                "ps_database_query_duration_ms",
                "Database query execution duration in milliseconds (excluding business logic)");

            BusinessLogicDurationMs = Meter.CreateHistogram<double>(
                "ps_business_logic_duration_ms",
                "Business logic execution duration in milliseconds (excluding database queries)");

            CacheHitsTotal = Meter.CreateCounter<long>(
                "ps_cache_hits_total",
                "Total count of successful cache hits by cache type");

            CacheMissesTotal = Meter.CreateCounter<long>(
                "ps_cache_misses_total",
                "Total count of cache misses requiring database lookup");

            DatabaseOperationsTotal = Meter.CreateCounter<long>(
                "ps_database_operations_total",
                "Total count of database operations by type (read/write)");

            _initialized = true;
        }
    }
}
