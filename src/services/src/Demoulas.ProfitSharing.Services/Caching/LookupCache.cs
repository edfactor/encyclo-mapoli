using System.Text.Json;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Caching;

/// <summary>
/// Generic lookup cache implementation using distributed caching with version-based invalidation.
/// Designed for small, frequently-accessed reference tables (StateTaxes, ProfitCodes, etc).
/// See DISTRIBUTED_CACHING_PATTERNS.md for architecture details.
/// </summary>
/// <typeparam name="TKey">The key type for cache lookups</typeparam>
/// <typeparam name="TValue">The value type being cached</typeparam>
/// <typeparam name="TEntity">The EF Core entity type to query</typeparam>
public class LookupCache<TKey, TValue, TEntity> : ILookupCache<TKey, TValue>
    where TKey : notnull
    where TEntity : class
{
    private readonly IDistributedCache _cache;
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly ILogger<LookupCache<TKey, TValue, TEntity>> _logger;
    private readonly string _lookupName;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _queryBuilder;
    private readonly Func<TEntity, TKey> _keySelector;
    private readonly Func<TEntity, TValue> _valueSelector;
    private readonly TimeSpan _absoluteExpiration;
    private readonly Func<ProfitSharingReadOnlyDbContext, IQueryable<TEntity>> _getDbSetFunc;

    // Version counter key (never expires)
    private string VersionKey => $"lookup:{_lookupName}:version";

    // Data cache key includes version for automatic invalidation
    private string GetDataKey(int version) => $"lookup:{_lookupName}:data:v{version}";

    /// <summary>
    /// Initializes a new instance of the lookup cache.
    /// </summary>
    /// <param name="cache">Distributed cache instance</param>
    /// <param name="contextFactory">EF Core context factory</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="lookupName">Unique name for this lookup (e.g., "StateTaxes")</param>
    /// <param name="queryBuilder">Function to build the query from DbSet</param>
    /// <param name="keySelector">Function to extract key from entity</param>
    /// <param name="valueSelector">Function to extract value from entity</param>
    /// <param name="getDbSetFunc">Function to get DbSet from read-only context (e.g., ctx => ctx.StateTaxes)</param>
    /// <param name="absoluteExpiration">Cache expiration time (default 1 hour)</param>
    public LookupCache(
        IDistributedCache cache,
        IProfitSharingDataContextFactory contextFactory,
        ILogger<LookupCache<TKey, TValue, TEntity>> logger,
        string lookupName,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        Func<TEntity, TKey> keySelector,
        Func<TEntity, TValue> valueSelector,
        Func<ProfitSharingReadOnlyDbContext, IQueryable<TEntity>> getDbSetFunc,
        TimeSpan? absoluteExpiration = null)
    {
        _cache = cache;
        _contextFactory = contextFactory;
        _logger = logger;
        _lookupName = lookupName;
        _queryBuilder = queryBuilder;
        _keySelector = keySelector;
        _valueSelector = valueSelector;
        _getDbSetFunc = getDbSetFunc;
        _absoluteExpiration = absoluteExpiration ?? TimeSpan.FromHours(1);
    }

    /// <inheritdoc />
    public async ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var allData = await GetAllAsync(cancellationToken);
        return allData.TryGetValue(key, out var value) ? value : default;
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyDictionary<TKey, TValue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Get current version (or initialize to 1)
            var version = await GetOrInitializeVersionAsync(cancellationToken);
            var dataKey = GetDataKey(version);

            // Try to get from cache
            var cachedBytes = await _cache.GetAsync(dataKey, cancellationToken);
            if (cachedBytes != null)
            {
                try
                {
                    var cachedData = JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(cachedBytes);
                    if (cachedData != null)
                    {
                        _logger.LogDebug("Cache hit for {LookupName} v{Version}", _lookupName, version);
                        return cachedData;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached {LookupName}, reloading from database", _lookupName);
                }
            }

            // Cache miss - load from database
            _logger.LogInformation("Cache miss for {LookupName} v{Version}, loading from database", _lookupName, version);
            var data = await LoadFromDatabaseAsync(cancellationToken);

            // Store in cache
            await SetCacheAsync(dataKey, data, cancellationToken);

            return data;
        }
        catch (Exception ex)
        {
            // On cache failure, degrade gracefully by querying database directly
            _logger.LogError(ex, "Cache operation failed for {LookupName}, falling back to database", _lookupName);
            return await LoadFromDatabaseAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task InvalidateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentVersion = await GetOrInitializeVersionAsync(cancellationToken);
            var newVersion = currentVersion + 1;

            // Increment version counter (no expiration - persists indefinitely)
            await _cache.SetStringAsync(
                VersionKey,
                newVersion.ToString(),
                new DistributedCacheEntryOptions(), // No expiration
                cancellationToken);

            _logger.LogInformation("Invalidated {LookupName} cache: v{OldVersion} → v{NewVersion}",
                _lookupName, currentVersion, newVersion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate {LookupName} cache", _lookupName);
            throw new InvalidOperationException($"Cache invalidation failed for {_lookupName}", ex);
        }
    }

    /// <summary>
    /// Gets the current cache version or initializes it to 1 if not present.
    /// </summary>
    private async ValueTask<int> GetOrInitializeVersionAsync(CancellationToken cancellationToken)
    {
        var versionString = await _cache.GetStringAsync(VersionKey, cancellationToken);
        if (!string.IsNullOrEmpty(versionString) && int.TryParse(versionString, out var version))
        {
            return version;
        }

        // Initialize version to 1
        await _cache.SetStringAsync(
            VersionKey,
            "1",
            new DistributedCacheEntryOptions(), // No expiration
            cancellationToken);

        return 1;
    }

    /// <summary>
    /// Loads all lookup data from the database using the provided read-only context.
    /// </summary>
    private async ValueTask<Dictionary<TKey, TValue>> LoadFromDatabaseAsync(CancellationToken cancellationToken)
    {
        return await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var dbSet = _getDbSetFunc(ctx);
            var query = _queryBuilder(dbSet);

            var entities = await query.ToListAsync(cancellationToken);

            return entities.ToDictionary(_keySelector, _valueSelector);
        }, cancellationToken);
    }

    /// <summary>
    /// Stores data in the distributed cache with configured expiration.
    /// </summary>
    private async Task SetCacheAsync(string key, Dictionary<TKey, TValue> data, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(data);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _absoluteExpiration
            };

            await _cache.SetAsync(key, json, options, cancellationToken);
            _logger.LogDebug("Cached {Count} {LookupName} entries", data.Count, _lookupName);
        }
        catch (Exception ex)
        {
            // Log but don't throw - cache set failures should not break the application
            _logger.LogWarning(ex, "Failed to cache {LookupName} data", _lookupName);
        }
    }
}
