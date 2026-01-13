using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;

namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Provides cache operations with automatic telemetry for cache hit/miss tracking.
/// Wraps IMemoryCache with telemetry-aware methods.
/// </summary>
public class CacheTelemetryWrapper
{
    private readonly IMemoryCache _cache;

    public CacheTelemetryWrapper(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Attempts to get a value from the cache and records hit/miss metrics.
    /// </summary>
    /// <typeparam name="T">Type of cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Retrieved value if found</param>
    /// <param name="cacheType">Descriptive cache type (e.g., "NavigationStatus", "CalendarYear")</param>
    /// <param name="endpointName">Name of the endpoint accessing cache (defaults to caller method)</param>
    /// <returns>True if value was found in cache (hit), false otherwise (miss)</returns>
    public bool TryGetValue<T>(string key, out T? value, string cacheType, [CallerMemberName] string endpointName = "Unknown")
    {
        var found = _cache.TryGetValue(key, out value);

        if (found)
        {
            // Record cache hit
            EndpointTelemetry.CacheHitsTotal.Add(1,
                new("cache_type", cacheType),
                new("endpoint", endpointName));
        }
        else
        {
            // Record cache miss
            EndpointTelemetry.CacheMissesTotal.Add(1,
                new("cache_type", cacheType),
                new("endpoint", endpointName));
        }

        return found;
    }

    /// <summary>
    /// Gets a value from cache or computes it using the factory function (with telemetry).
    /// </summary>
    /// <typeparam name="T">Type of cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="factory">Function to compute value on cache miss</param>
    /// <param name="cacheType">Descriptive cache type</param>
    /// <param name="expirationMinutes">Cache expiration in minutes (default 15)</param>
    /// <param name="endpointName">Name of the endpoint accessing cache</param>
    /// <returns>Cached or computed value</returns>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        string cacheType,
        int expirationMinutes = 15,
        [CallerMemberName] string endpointName = "Unknown")
    {
        if (_cache.TryGetValue<T>(key, out var cachedValue))
        {
            // Record cache hit
            EndpointTelemetry.CacheHitsTotal.Add(1,
                new("cache_type", cacheType),
                new("endpoint", endpointName));

            return cachedValue!;
        }

        // Record cache miss
        EndpointTelemetry.CacheMissesTotal.Add(1,
            new("cache_type", cacheType),
            new("endpoint", endpointName));

        // Compute value and cache it
        var value = await factory();
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(expirationMinutes));

        _cache.Set(key, value, cacheEntryOptions);

        return value;
    }

    /// <summary>
    /// Sets a value in the cache.
    /// </summary>
    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (expiration.HasValue)
        {
            _cache.Set(key, value, expiration.Value);
        }
        else
        {
            _cache.Set(key, value, TimeSpan.FromMinutes(15));
        }
    }

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}
