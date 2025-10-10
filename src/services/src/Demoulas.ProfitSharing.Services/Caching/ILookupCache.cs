namespace Demoulas.ProfitSharing.Services.Caching;

/// <summary>
/// Generic lookup cache interface for frequently accessed reference data.
/// Uses distributed caching with version-based invalidation.
/// </summary>
/// <typeparam name="TKey">The key type for cache lookups</typeparam>
/// <typeparam name="TValue">The value type being cached</typeparam>
public interface ILookupCache<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Gets a cached value by key, loading from database if not cached.
    /// </summary>
    /// <param name="key">The lookup key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached value or null if not found</returns>
    ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all cached values, loading from database if not cached.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of all cached lookup values</returns>
    ValueTask<IReadOnlyDictionary<TKey, TValue>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cache by incrementing the version counter.
    /// Does not remove data - next access will reload with new version key.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InvalidateAsync(CancellationToken cancellationToken = default);
}
