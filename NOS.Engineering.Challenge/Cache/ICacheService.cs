﻿namespace NOS.Engineering.Challenge.Cache;

public interface ICacheService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> dataFunc, TimeSpan? slidingExpiration = null);
    Task SetAsync<T>(Guid id, T item);
    Task RemoveAsync(Guid id);
}
