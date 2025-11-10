namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Extension methods for recording cache telemetry manually when not using CacheTelemetryWrapper.
/// </summary>
public static class CacheTelemetryExtensions
{
    /// <summary>
    /// Records a cache hit metric.
    /// </summary>
    public static void RecordCacheHit(string cacheType, string endpointName)
    {
        EndpointTelemetry.CacheHitsTotal.Add(1,
            new("cache_type", cacheType),
            new("endpoint", endpointName));
    }

    /// <summary>
    /// Records a cache miss metric.
    /// </summary>
    public static void RecordCacheMiss(string cacheType, string endpointName)
    {
        EndpointTelemetry.CacheMissesTotal.Add(1,
            new("cache_type", cacheType),
            new("endpoint", endpointName));
    }
}