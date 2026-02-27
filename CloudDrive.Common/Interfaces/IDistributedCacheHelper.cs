using Microsoft.Extensions.Caching.Distributed;

namespace CloudDrive.Common.Interfaces
{
    public interface IDistributedCacheHelper
    {
        TResult? GetResult<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds = 60);
        Task<TResult?> GetResultAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds = 60);
        void Remove(string cacheKey);
        Task RemoveAsync(string cacheKey);
    }
}
