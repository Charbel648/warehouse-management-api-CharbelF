using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Common.Caching;

namespace Warehouse.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        string? cachedValue = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrWhiteSpace(cachedValue))
        {
            _logger.LogInformation("Cache miss for key {CacheKey}", key);
            return default;
        }

        _logger.LogInformation("Cache hit for key {CacheKey}", key);

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        string serializedValue = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            serializedValue,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            },
            cancellationToken);

        _logger.LogInformation(
            "Cache refreshed for key {CacheKey} with expiration {Expiration}",
            key,
            expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(key, cancellationToken);

        _logger.LogInformation("Cache invalidated for key {CacheKey}", key);
    }
}

