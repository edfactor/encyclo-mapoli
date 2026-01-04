---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# Demoulas.Common.Caching - Distributed Caching with Hosted Services

**Package:** `Demoulas.Common.Caching`  
**Namespace:** `Demoulas.Common.Caching`

This package provides a base implementation for distributed caching using `IDistributedCache` with automatic refresh capabilities through hosted services. Ideal for caching frequently-accessed data that changes periodically.

## Table of Contents

1. [BaseCacheHostedService](#basecachehostedservice)
2. [CacheDataDictionary](#cachedatadictionary)
3. [Event Handling](#event-handling)
4. [Telemetry and Monitoring](#telemetry-and-monitoring)
5. [Usage Examples](#usage-examples)

---

## BaseCacheHostedService

**Namespace:** `Demoulas.Common.Caching`  
**Base Class:** `BaseCacheHostedService<T>`

### Overview

Abstract base class for implementing background caching services that automatically refresh cached data at configurable intervals. Provides built-in:

-   **Automatic refresh** on a timer (configurable seconds)
-   **Delta detection** - only updates changed items
-   **Event notifications** - raises events when cache items are added, updated, or removed
-   **OpenTelemetry metrics** - tracks refresh counts, failures, and durations
-   **Thread-safe operations** with proper cancellation support

### Required Abstract Members

Implementations must override:

```csharp
protected abstract string BaseKeyName { get; }
protected abstract ushort RefreshSeconds { get; set; }

public abstract Task<IEnumerable<T>> GetInitialDataToCacheAsync(CancellationToken cancellation = default);
public abstract Task<IEnumerable<T>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default);
```

### Properties

#### BaseKeyName

The cache key prefix for this service. Used to namespace cache entries.

**Example:** `"PayClassification"`, `"StoreList"`, `"ProductCatalog"`

#### RefreshSeconds

How often the cache refreshes in seconds. Default implementations use 3600 (hourly).

**Common values:**

-   `300` - 5 minutes (frequently changing data)
-   `1800` - 30 minutes (moderate updates)
-   `3600` - 1 hour (stable data)
-   `86400` - 24 hours (rarely changing)

#### CacheVersion

Static property controlling cache key format. Default is `2`. Increment to invalidate all existing cache entries across deployments.

### Methods

#### GetInitialDataToCacheAsync

Called on service startup to populate the cache with initial data.

**Returns:** Complete dataset to cache.

#### GetDataToUpdateCacheAsync

Called on each refresh cycle. Receives current `CacheDataDictionary` to support delta detection.

**Parameters:**

-   `cdd` - Current cached data dictionary for comparison
-   `cancellation` - Cancellation token

**Returns:** Updated dataset. Framework automatically detects adds, updates, and removals.

---

## CacheDataDictionary

**Namespace:** `Demoulas.Common.Caching`

Thread-safe dictionary wrapper for managing cached items. Keyed by cache key string.

### Methods

**`ContainsKey(string key)`**  
Checks if a cache key exists.

**`TryGetValue(string key, out T value)`**  
Attempts to retrieve a cached value.

**`GetAllValues()`**  
Returns all cached values as an enumerable.

**`Remove(string key)`**  
Removes an item from the cache dictionary.

**`Add(string key, T value)`**  
Adds a new item to the cache.

**`Update(string key, T value)`**  
Updates an existing cache entry.

---

## Event Handling

**Event:** `CacheItemChanged`  
**Type:** `EventHandler<CacheItemChangedEventArgs>`

Raised whenever a cache item is added, updated, or removed.

### CacheItemChangedEventArgs

```csharp
public class CacheItemChangedEventArgs : EventArgs
{
    public string Key { get; set; }
    public string ChangeType { get; set; } // "Added", "Updated", "Removed"
}
```

### Example Event Subscription

```csharp
public class MyHostedService : BaseCacheHostedService<MyDataDto>
{
    public MyHostedService(/* dependencies */) : base(/* parameters */)
    {
        CacheItemChanged += OnCacheItemChanged;
    }

    private void OnCacheItemChanged(object? sender, CacheItemChangedEventArgs e)
    {
        _logger.LogInformation("Cache item {Key} was {ChangeType}", e.Key, e.ChangeType);

        // React to cache changes (e.g., notify connected clients)
    }
}
```

---

## Telemetry and Monitoring

The package emits OpenTelemetry metrics for cache operations:

### Metrics

| Metric Name              | Type      | Description                                 |
| ------------------------ | --------- | ------------------------------------------- |
| `cache.refresh.count`    | Counter   | Total number of cache refresh operations    |
| `cache.refresh.failures` | Counter   | Number of failed refresh attempts           |
| `cache.refresh.duration` | Histogram | Time taken for each refresh in milliseconds |
| `cache.items.updated`    | Counter   | Per-item update count                       |
| `cache.items.removed`    | Counter   | Per-item removal count                      |

### ActivitySource

**Name:** `Demoulas.Common.Caching`

Traces cache refresh operations with spans for:

-   Initial cache population
-   Scheduled refreshes
-   Delta detection operations

---

## Usage Examples

### Example 1: Basic Caching Service

```csharp
using Demoulas.Common.Caching;
using Microsoft.Extensions.Caching.Distributed;

public sealed class PayClassificationHostedService
    : BaseCacheHostedService<PayClassificationDto>
{
    private readonly IPayrollContextFactory _contextFactory;

    protected override string BaseKeyName => "PayClassification";
    protected override ushort RefreshSeconds { get; set; } = 3600; // Hourly

    public PayClassificationHostedService(
        IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IPayrollContextFactory contextFactory)
        : base(hostEnvironment, distributedCache)
    {
        _contextFactory = contextFactory;
    }

    public override async Task<IEnumerable<PayClassificationDto>> GetInitialDataToCacheAsync(
        CancellationToken cancellation = default)
    {
        return await FetchAllPayClassifications(cancellation);
    }

    public override async Task<IEnumerable<PayClassificationDto>> GetDataToUpdateCacheAsync(
        CacheDataDictionary cdd,
        CancellationToken cancellation = default)
    {
        // Can use 'cdd' to check for changes before querying
        return await FetchAllPayClassifications(cancellation);
    }

    private async Task<IEnumerable<PayClassificationDto>> FetchAllPayClassifications(
        CancellationToken cancellationToken)
    {
        return await _contextFactory.UseReadOnlyContext(context =>
        {
            return context.PayClassifications
                .Select(c => new PayClassificationDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description
                })
                .ToListAsync(cancellationToken);
        });
    }
}
```

### Example 2: Service Registration

```csharp
// Program.cs or Startup.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddHostedService<PayClassificationHostedService>();
```

### Example 3: Delta Detection with Change Tracking

```csharp
public override async Task<IEnumerable<StoreDto>> GetDataToUpdateCacheAsync(
    CacheDataDictionary cdd,
    CancellationToken cancellation = default)
{
    // Get only stores modified since last refresh
    var lastRefresh = GetLastRefreshTime();

    return await _contextFactory.UseReadOnlyContext(context =>
    {
        return context.Stores
            .Where(s => s.ModifiedDate > lastRefresh)
            .Select(s => new StoreDto
            {
                StoreId = s.StoreId,
                Name = s.Name,
                ModifiedDate = s.ModifiedDate
            })
            .ToListAsync(cancellation);
    });
}
```

### Example 4: Consuming Cached Data

```csharp
public class PayrollService
{
    private readonly IDistributedCache _cache;

    public PayrollService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<PayClassificationDto?> GetPayClassificationAsync(string code)
    {
        var cacheKey = $"PayClassification:{code}";
        var cachedJson = await _cache.GetStringAsync(cacheKey);

        if (cachedJson != null)
        {
            return JsonSerializer.Deserialize<PayClassificationDto>(cachedJson);
        }

        return null;
    }
}
```

---

## Best Practices

### Cache Key Design

1. **Use namespaced keys:** `BaseKeyName:UniqueId`
2. **Keep keys concise** but descriptive
3. **Avoid PII** in cache keys
4. **Use consistent casing** (e.g., PascalCase)

### Refresh Interval Selection

-   **Frequent changes:** 5-15 minutes
-   **Moderate stability:** 30-60 minutes
-   **Rarely changes:** 1-24 hours
-   **Consider data volume** - larger datasets may need longer intervals

### Delta Detection Strategy

1. **Timestamp-based:** Query by `ModifiedDate > lastRefresh`
2. **Version-based:** Use version numbers or ETags
3. **Full refresh:** For small datasets (<1000 items)

### Error Handling

```csharp
public override async Task<IEnumerable<T>> GetDataToUpdateCacheAsync(
    CacheDataDictionary cdd,
    CancellationToken cancellation = default)
{
    try
    {
        return await FetchData(cancellation);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to refresh cache for {BaseKeyName}", BaseKeyName);

        // Return empty to skip this refresh cycle
        return Enumerable.Empty<T>();
    }
}
```

### Performance Considerations

-   Use **`IAsyncEnumerable<T>`** for large datasets
-   Implement **projection** in queries (don't materialize full entities)
-   Consider **compression** for large cached objects
-   Monitor **cache hit rates** via telemetry

---

## Multi-Targeting Support

This package supports **net8.0, net9.0, and net10.0**. No framework-specific considerations.

---

## Related Documentation

-   [Demoulas.Common.Contracts Instructions](./demoulas.common.contracts.instructions.md) - Shared interfaces and DTOs
-   [Demoulas.Common.Data Instructions](./demoulas.common.data.instructions.md) - Database context patterns
-   [Code Review Instructions](./code-review.instructions.md) - Review standards
