# Distributed Caching Patterns

Complete reference for implementing distributed caching patterns in the Profit Sharing application using `IDistributedCache`.

## Overview

All application caching MUST use `IDistributedCache` (NOT `IMemoryCache`) to support Redis in production and `MemoryDistributedCache` in unit tests. This ensures consistency across environments and enables horizontal scaling.

**Key Benefits**:
- Redis backing in production (configured via Aspire)
- Seamless unit testing with `MemoryDistributedCache`
- Consistent API across all cache implementations
- Horizontal scaling support

## Core Patterns

### Data Serialization

Always serialize cache data to JSON using `System.Text.Json`:

```csharp
// Writing to cache
var data = await LoadFromDatabaseAsync(ct);
var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(data);
await _distributedCache.SetAsync(cacheKey, jsonBytes, options, ct);

// Reading from cache
var cachedBytes = await _distributedCache.GetAsync(cacheKey, ct);
if (cachedBytes != null)
{
    var cachedData = JsonSerializer.Deserialize<MyDto>(cachedBytes);
    return cachedData;
}
```

### Cache Key Design

Use descriptive prefixes and low-cardinality identifiers:

**Good Examples**:
- `navigation-status-all`
- `frozen-demographics-v{year}`
- `navigation-tree-v{version}-ROLE1|ROLE2` (sorted roles)
- `calendar-year-{year}`

**Avoid**:
- High-cardinality keys: `user-data-{userId}` (thousands of unique keys)
- Unsorted combinations: `roles-ADMIN|USER` vs `roles-USER|ADMIN` (creates duplicates)

### Cache Expiration

Always set both absolute and sliding expiration:

```csharp
var options = new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // Hard limit
    SlidingExpiration = TimeSpan.FromMinutes(15)                 // Extends on access
};
await _distributedCache.SetAsync(cacheKey, jsonBytes, options, ct);
```

**Guidelines**:
- **Absolute expiration**: Maximum time data can remain in cache (e.g., 30 min for navigation data)
- **Sliding expiration**: Resets on each access to keep frequently-used data fresh (e.g., 15 min)
- **Reference data**: Longer expiration (1-2 hours)
- **User-specific data**: Shorter expiration (15-30 minutes)

## Cache Invalidation Patterns

### Simple Removal (Single Key)

For single-key invalidation (e.g., status flags, simple lookups):

```csharp
public async Task InvalidateNavigationStatusAsync(CancellationToken ct)
{
    const string cacheKey = "navigation-status-all";
    await _distributedCache.RemoveAsync(cacheKey, ct);
    _logger?.LogInformation("Navigation status cache invalidated");
}
```

**Use When**:
- Single cache key needs invalidation
- No related keys need invalidation
- Simple on/off state or lookup data

### Version-Based Invalidation (PREFERRED for Multi-Key Scenarios)

For invalidating multiple related cache keys instantly:

**Why Version-Based?**:
- `IDistributedCache` has NO pattern-matching delete (no `RemoveByPrefix`)
- Incrementing version makes ALL old keys unreachable
- Old keys expire naturally based on TTL
- Works with any `IDistributedCache` provider (Redis, SQL Server, in-memory)

**Implementation Pattern**:

```csharp
// 1. Get or create version counter
private async Task<int> GetCacheVersionAsync(CancellationToken ct)
{
    var versionBytes = await _distributedCache.GetAsync("my-cache-version", ct);
    return versionBytes != null ? BitConverter.ToInt32(versionBytes) : 0;
}

// 2. Build cache key with version
public async Task<MyDto> GetDataAsync(string identifier, CancellationToken ct)
{
    var version = await GetCacheVersionAsync(ct);
    var cacheKey = $"my-data-v{version}-{identifier}";
    
    // Try cache lookup
    var cachedBytes = await _distributedCache.GetAsync(cacheKey, ct);
    if (cachedBytes != null)
    {
        var cachedData = JsonSerializer.Deserialize<MyDto>(cachedBytes);
        _logger?.LogDebug("Data loaded from cache (version {Version})", version);
        return cachedData;
    }
    
    // Load from database, serialize, cache
    var data = await LoadFromDatabaseAsync(identifier, ct);
    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(data);
    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        SlidingExpiration = TimeSpan.FromMinutes(15)
    };
    await _distributedCache.SetAsync(cacheKey, jsonBytes, options, ct);
    
    _logger?.LogInformation("Data loaded from database and cached (version {Version})", version);
    return data;
}

// 3. Invalidate ALL versions by incrementing counter
public async Task InvalidateAllCachesAsync(CancellationToken ct)
{
    var oldVersion = await GetCacheVersionAsync(ct);
    var newVersion = oldVersion + 1;
    
    // Store new version (NO EXPIRATION - must persist)
    await _distributedCache.SetAsync(
        "my-cache-version", 
        BitConverter.GetBytes(newVersion), 
        ct);
    
    _logger?.LogInformation(
        "Cache version incremented from {OldVersion} to {NewVersion} - all cached data invalidated", 
        oldVersion, 
        newVersion);
}
```

**Critical Notes**:
- Version counter MUST NOT have expiration (store indefinitely)
- All cache reads MUST include version in key
- Old versioned keys expire naturally via TTL
- Instant invalidation with single write operation

### Role-Based Cache Keys

When caching data that varies by user roles:

```csharp
public async Task<NavigationTree> GetNavigationAsync(
    IEnumerable<string> roles, 
    CancellationToken ct)
{
    var version = await GetCacheVersionAsync(ct);
    
    // Sort roles for consistent key generation
    var sortedRoles = string.Join("|", roles.OrderBy(r => r));
    var cacheKey = $"navigation-tree-v{version}-{sortedRoles}";
    
    // Cache lookup and storage logic...
}
```

**Guidelines**:
- Always sort role names before building key
- Use pipe `|` separator for readability
- Include version prefix for invalidation
- Document role combinations in logs

## Unit Testing Patterns

### Test Setup with MemoryDistributedCache

```csharp
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class MyServiceTests
{
    private readonly IDistributedCache _distributedCache;
    private readonly MyService _service;
    
    public MyServiceTests()
    {
        // Create in-memory distributed cache for testing
        var options = Options.Create(new MemoryDistributedCacheOptions());
        _distributedCache = new MemoryDistributedCache(options);
        
        // Inject into service
        _service = new MyService(_distributedCache, /* other dependencies */);
    }
    
    [Fact]
    public async Task GetData_SecondCall_ReturnsCachedData()
    {
        // Arrange
        var identifier = "test-123";
        
        // Act - First call (cache miss, loads from database)
        var result1 = await _service.GetDataAsync(identifier, CancellationToken.None);
        
        // Act - Second call (cache hit, no database call)
        var result2 = await _service.GetDataAsync(identifier, CancellationToken.None);
        
        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Id.ShouldBe(result2.Id);
        // Verify database was only called once (mock verification)
    }
    
    [Fact]
    public async Task InvalidateCache_RemovesAllVersionedKeys()
    {
        // Arrange - Populate cache
        await _service.GetDataAsync("test-1", CancellationToken.None);
        await _service.GetDataAsync("test-2", CancellationToken.None);
        
        // Act - Invalidate (increments version)
        await _service.InvalidateAllCachesAsync(CancellationToken.None);
        
        // Act - Retrieve again (should miss cache with new version)
        var result = await _service.GetDataAsync("test-1", CancellationToken.None);
        
        // Assert - Verify database was called again (cache miss)
        result.ShouldNotBeNull();
    }
}
```

### DI Registration for Tests

```csharp
// In test project's service collection setup
services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
```

**Benefits**:
- No code changes needed in services
- Tests run transparently with in-memory cache
- Same API as production Redis cache
- Fast test execution

## Logging Patterns

### Cache Hit Logging

```csharp
_logger?.LogDebug(
    "Data loaded from cache (version {Version}, key: {CacheKey})", 
    version, 
    cacheKey);
```

### Cache Miss Logging

```csharp
_logger?.LogInformation(
    "Cache miss - loading {Count} records from database (key: {CacheKey})", 
    recordCount, 
    cacheKey);
```

### Cache Invalidation Logging

```csharp
_logger?.LogInformation(
    "Cache invalidated for {Reason} (ID: {EntityId}, version: {OldVersion} → {NewVersion})", 
    "navigation update", 
    navigationId, 
    oldVersion, 
    newVersion);
```

### Cache Error Logging

```csharp
try
{
    var cachedBytes = await _distributedCache.GetAsync(cacheKey, ct);
    // Process cached data...
}
catch (Exception ex)
{
    _logger?.LogError(
        ex, 
        "Cache read failed for key {CacheKey} - falling back to database", 
        cacheKey);
    // Degrade gracefully - load from database
}
```

## Error Handling Patterns

### Graceful Degradation

Cache failures should NOT fail operations:

```csharp
public async Task<MyDto> GetDataAsync(string identifier, CancellationToken ct)
{
    MyDto? cachedData = null;
    
    // Try cache (non-critical operation)
    try
    {
        var version = await GetCacheVersionAsync(ct);
        var cacheKey = $"my-data-v{version}-{identifier}";
        var cachedBytes = await _distributedCache.GetAsync(cacheKey, ct);
        
        if (cachedBytes != null)
        {
            cachedData = JsonSerializer.Deserialize<MyDto>(cachedBytes);
            _logger?.LogDebug("Cache hit for {Identifier}", identifier);
            return cachedData;
        }
    }
    catch (Exception ex)
    {
        _logger?.LogError(ex, "Cache read failed for {Identifier} - continuing without cache", identifier);
        // Continue to database load
    }
    
    // Load from database (critical operation)
    var data = await LoadFromDatabaseAsync(identifier, ct);
    
    // Try to populate cache (best effort)
    try
    {
        var version = await GetCacheVersionAsync(ct);
        var cacheKey = $"my-data-v{version}-{identifier}";
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(15)
        };
        await _distributedCache.SetAsync(cacheKey, jsonBytes, options, ct);
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Cache write failed for {Identifier} - operation succeeded without caching", identifier);
        // Don't fail operation because of cache write failure
    }
    
    return data;
}
```

### Cache Availability Checks

For optional cache warming or pre-loading scenarios:

```csharp
private async Task<bool> IsCacheAvailableAsync(CancellationToken ct)
{
    try
    {
        var testKey = "cache-health-check";
        await _distributedCache.SetAsync(testKey, new byte[] { 1 }, ct);
        await _distributedCache.RemoveAsync(testKey, ct);
        return true;
    }
    catch
    {
        return false;
    }
}
```

## Reference Implementations

### NavigationService (Version-Based Invalidation)

See `NavigationService.GetNavigation()` for complete implementation of:
- Version-based invalidation with role-based keys
- Sorted role combination keys
- Graceful cache degradation
- Comprehensive logging

**Path**: `src/services/src/Demoulas.ProfitSharing.Services/NavigationService.cs`

### NavigationService (Simple Key)

See `NavigationService.GetNavigationStatus()` for simple single-key pattern:
- Direct key-based caching
- Simple removal invalidation
- Status flag caching

### FrozenService (Year-Based Keys)

See `FrozenService` for year-scoped caching:
- Year-based cache keys
- Simple removal for year-specific data
- Business-rule-driven expiration

**Path**: `src/services/src/Demoulas.ProfitSharing.Services/FrozenService.cs`

## Configuration

### appsettings.json

```json
{
  "Caching": {
    "DefaultAbsoluteExpirationMinutes": 30,
    "DefaultSlidingExpirationMinutes": 15,
    "NavigationCacheMinutes": 60,
    "ReferenceCacheMinutes": 120
  }
}
```

### Aspire Configuration

Redis is configured via Aspire host - no manual connection string management required.

**AppHost configuration**: `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs`

## Best Practices Summary

### DO:
- ✅ Use `IDistributedCache` for all application caching
- ✅ Serialize data to JSON via `System.Text.Json`
- ✅ Set both absolute and sliding expiration
- ✅ Use version-based invalidation for multi-key scenarios
- ✅ Sort role names before building cache keys
- ✅ Degrade gracefully on cache failures
- ✅ Log cache hits (Debug), misses (Information), and invalidations (Information)
- ✅ Test caching logic with `MemoryDistributedCache`
- ✅ Keep cache keys low-cardinality (avoid user IDs)

### DO NOT:
- ❌ Use `IMemoryCache` for application caching
- ❌ Attempt pattern-based cache deletion (`RemoveByPrefix` doesn't exist)
- ❌ Store version counters with expiration
- ❌ Include high-cardinality data in cache keys (e.g., individual user IDs)
- ❌ Fail operations when cache operations fail
- ❌ Hardcode cache expiration times (use configuration)
- ❌ Use unsorted role combinations in keys
- ❌ Cache sensitive PII without encryption

## Related Documentation

- `TELEMETRY_GUIDE.md` - Observability and monitoring patterns
- `NAVIGATION_CACHING_COMPLETE.md` - Detailed navigation caching example
- `UNIT_TESTS_IDISTRIBUTEDCACHE_COMPLETE.md` - Comprehensive unit testing guide
- `DISTRIBUTED_CACHE_MIGRATION_SUMMARY.md` - Migration guide from IMemoryCache

## Troubleshooting

### Cache Not Invalidating

**Problem**: Data still returns old values after invalidation.

**Solution**: Verify version counter is being incremented and all cache reads include version in key.

```csharp
// Check version in logs
_logger?.LogInformation("Current cache version: {Version}", version);
```

### High Memory Usage

**Problem**: Cache consuming too much memory in Redis.

**Solution**: 
1. Reduce expiration times
2. Check for high-cardinality keys
3. Review cache key patterns for duplicates (unsorted combinations)

### Tests Failing Intermittently

**Problem**: Cache-related tests fail randomly.

**Solution**:
1. Ensure `MemoryDistributedCache` is registered as singleton
2. Don't share cache instance between unrelated tests
3. Clear cache between test cases if needed

### Cache Miss Rate Too High

**Problem**: Cache hit rate is low despite caching.

**Solution**:
1. Check cache key generation for consistency
2. Verify expiration times aren't too short
3. Ensure version isn't being incremented too frequently
4. Review logs for cache errors causing fallback to database

---

**Last Updated**: October 2025  
**Maintained By**: Platform Team  
**Questions**: Contact #platform-engineering
