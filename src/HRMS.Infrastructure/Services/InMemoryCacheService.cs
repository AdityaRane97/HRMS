using HRMS.Application.Contracts;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory implementation of cache service for Phase 1 development.
/// Useful for local testing without Redis dependency.
/// Will be replaced with Redis implementation for production.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly Dictionary<string, CacheEntry> _cache = new();
    private readonly object _lockObject = new();

    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        lock (_lockObject)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                // Check expiration
                if (entry.ExpiresAt.HasValue && entry.ExpiresAt <= DateTime.UtcNow)
                {
                    _cache.Remove(key);
                    return Task.FromResult<T?>(null);
                }

                return Task.FromResult(entry.Value as T);
            }
        }

        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        lock (_lockObject)
        {
            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null
            };

            _cache[key] = entry;
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            _cache.Remove(key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            var keysToRemove = _cache.Keys
                .Where(k => k.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in keysToRemove)
                _cache.Remove(key);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                // Check expiration
                if (entry.ExpiresAt.HasValue && entry.ExpiresAt <= DateTime.UtcNow)
                {
                    _cache.Remove(key);
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
