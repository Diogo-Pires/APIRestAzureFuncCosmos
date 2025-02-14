using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Application.Interfaces;

namespace Infrastructure.Cache;

public class HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache) : IHybridCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> fetchFromDb) where T : class
    {
        // Try to get L1 cache
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        // Try to get L2 cache(Redis)
        var redisData = await _distributedCache.GetStringAsync(key);
        if (redisData != null)
        {
            cachedValue = JsonSerializer.Deserialize<T>(redisData);
            _memoryCache.Set(key, cachedValue, _cacheDuration);

            return cachedValue;
        }

        // If nothing found, go to the DB
        cachedValue = await fetchFromDb();
        if (cachedValue != null)
        {
            var serializedData = JsonSerializer.Serialize(cachedValue);
            await _distributedCache.SetStringAsync(key, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });

            _memoryCache.Set(key, cachedValue, _cacheDuration);
        }

        return cachedValue;
    }

    public async Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        await _distributedCache.RemoveAsync(key);
    }
}