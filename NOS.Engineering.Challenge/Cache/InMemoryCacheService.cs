
using Microsoft.Extensions.Caching.Memory;

namespace NOS.Engineering.Challenge.Cache;

public class InMemoryCacheService(IMemoryCache _cache) : ICacheService
{
    public async Task<T> GetOrSetAsync<T>(Guid key, Func<Task<T>> dataFunc, TimeSpan? slidingExpiration = null)
    {
        if(!_cache.TryGetValue(key, out T cachedData))
        {
            cachedData = await dataFunc();

            MemoryCacheEntryOptions cacheOptions = new()
            {
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromSeconds(90),
            };

            _cache.Set(key, cachedData, cacheOptions);
        }

        return cachedData;
    }

    public async Task SetAsync<T>(Guid id, T item)
    {
        _cache.Set(id, item);

        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id)
    {
        _cache.Remove(id);

        await Task.CompletedTask;
    }
}
