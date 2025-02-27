using Application.Interfaces;

namespace Application.Services;

public abstract class BaseHybridCacheService
{
    protected const string BASE_CACHEKEY_ALL = "all";

    protected abstract string CacheKey { get; }

    protected async Task ClearAllRequestFromCacheAsync(IHybridCacheService hybridCacheService) =>
        await hybridCacheService.RemoveAsync($"{CacheKey}{BASE_CACHEKEY_ALL}");
}