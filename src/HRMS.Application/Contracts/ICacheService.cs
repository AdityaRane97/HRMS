namespace HRMS.Application.Contracts;

/// <summary>
/// Cloud-neutral caching abstraction.
/// Supports Redis, Azure Cache for Redis, AWS ElastiCache, and in-memory caching.
/// Cache is optional and implementation-dependent on deployment.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get a value from cache.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Set a value in cache.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Remove a value from cache.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove all values matching a pattern.
    /// Useful for invalidating related cache entries.
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a key exists in cache.
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
