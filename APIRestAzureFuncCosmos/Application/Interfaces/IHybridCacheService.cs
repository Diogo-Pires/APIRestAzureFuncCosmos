namespace Application.Interfaces;

public interface IHybridCacheService
{
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> fetchFromDb) where T : class;
    Task RemoveAsync(string key);
}