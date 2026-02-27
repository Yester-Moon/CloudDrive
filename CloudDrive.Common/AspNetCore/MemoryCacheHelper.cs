using CloudDrive.Common.Extensions;
using CloudDrive.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Common.AspNetCore
{
    public class MemoryCacheHelper : IMemoryCacheHelper
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheHelper(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 禁用IEnumerable、IQueryable等有延迟执行的类型作为缓存值，避免出现缓存中存储了一个查询对象，而不是查询结果的情况。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ValidateValueType<TResult>()
        {
            Type typeResult = typeof(TResult);
            if (typeResult.IsValueType)
            {
                typeResult = typeResult.GetGenericTypeDefinition();
            }
            if(typeResult == typeof(IEnumerable<>) || typeResult == typeof(IEnumerable)
                || typeResult == typeof(IAsyncEnumerable<TResult>)
                || typeResult == typeof(IQueryable<TResult>) || typeResult == typeof(IQueryable))
            {
                throw new InvalidOperationException($"TResult of {typeResult} is not allowed, please use List<T> or T[] instead.");
            }
        }

        private static void InitCacheEntry(ICacheEntry entry, int baseExpireSeconds)
        {
            //过期时间.Random.Shared 是.NET6新增的
            double sec = Random.Shared.NextDouble(baseExpireSeconds, baseExpireSeconds * 2);
            TimeSpan expiration = TimeSpan.FromSeconds(sec);
            entry.AbsoluteExpirationRelativeToNow = expiration;
        }

        public TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 60)
        {
            ValidateValueType<TResult>();
            if(!_cache.TryGetValue(cacheKey, out TResult? result))
            {
                using ICacheEntry entry = _cache.CreateEntry(cacheKey);
                InitCacheEntry(entry, expireSeconds);
                result = valueFactory(entry);
                entry.Value = result;
            }
            return result;
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            ValidateValueType<TResult>();
            if (!_cache.TryGetValue(cacheKey, out TResult? result))
            {
                using ICacheEntry entry = _cache.CreateEntry(cacheKey);
                InitCacheEntry(entry, expireSeconds);
                result = (await valueFactory(entry))!;
                entry.Value = result;
            }
            return result;
        }

        public void Remove(string cacheKey)
        {
           _cache.Remove(cacheKey);
        }

    }
}
