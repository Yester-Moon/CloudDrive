using CloudDrive.Common.Extensions;
using CloudDrive.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace CloudDrive.Common.AspNetCore
{
    public class DistributedCacheHelper : IDistributedCacheHelper
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheHelper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// 设定过期时间，随机在 baseExpireSeconds 到 baseExpireSeconds*2 之间
        /// </summary>
        /// <param name="baseExpireSeconds"></param>
        /// <returns></returns>
        private static DistributedCacheEntryOptions CreateOptions(int baseExpireSeconds)
        {
            //过期时间.Random.Shared 是.NET6新增的
            double sec = Random.Shared.NextDouble(baseExpireSeconds, baseExpireSeconds * 2);
            TimeSpan expiration = TimeSpan.FromSeconds(sec);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = expiration;
            return options;
        }

        public TResult? GetResult<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds = 60)
        {
            string? jsonStr = _distributedCache.GetString(cacheKey);
            if (string.IsNullOrWhiteSpace(jsonStr))
            {
                DistributedCacheEntryOptions options = CreateOptions(expireSeconds);
                TResult? result = valueFactory(options);
                string jsonOfResult = JsonSerializer.Serialize(result, typeof(TResult));//null会被json序列化为字符串"null"，所以可以防范“缓存穿透”
                _distributedCache.SetString(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                _distributedCache.Refresh(cacheKey);//刷新，以便于滑动过期时间延期
                return JsonSerializer.Deserialize<TResult>(jsonStr)!;
            }

        }

        public async Task<TResult?> GetResultAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            string? jsonStr = await _distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                var options = CreateOptions(expireSeconds);
                TResult? result = await valueFactory(options);
                string jsonOfResult = JsonSerializer.Serialize(result,
                    typeof(TResult));
                await _distributedCache.SetStringAsync(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                await _distributedCache.RefreshAsync(cacheKey);
                return JsonSerializer.Deserialize<TResult>(jsonStr)!;
            }
        }

        public void Remove(string cacheKey)
        {
            _distributedCache.Remove(cacheKey);
        }

        public Task RemoveAsync(string cacheKey)
        {
            return _distributedCache.RemoveAsync(cacheKey);
        }
    }
}
