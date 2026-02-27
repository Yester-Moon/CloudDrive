using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Common.Interfaces
{
    public interface IMemoryCacheHelper
    {
        TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 60);

        Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 60);

        void Remove(string cacheKey);
    }
}
