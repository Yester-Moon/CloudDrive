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

        /// <summary>
        /// 异步版本的GetOrCreate，适用于valueFactory中有异步操作的场景。
        /// ICacheEntry本身不支持异步，但我们可以在valueFactory中执行异步操作，并在完成后设置entry.Value,以确保缓存项正确存储结果。
        /// ICacheEntry用于设置缓存项的过期时间等属性，而valueFactory负责生成缓存值，可以包含异步逻辑。GetOrCreateAsync方法会等待valueFactory完成后再将结果存储到缓存中。
        /// 在Func委托暴露ICacheEntry参数的同时，允许调用者在生成缓存值时访问和配置缓存项的属性（如过期时间），这使得GetOrCreateAsync方法非常灵活，适用于各种异步数据获取和缓存场景。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="valueFactory"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
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
