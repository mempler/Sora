using System;
using Microsoft.Extensions.Caching.Memory;

namespace Sora.Framework.Allocation
{
    public class Cache
    {
        private readonly IMemoryCache _memoryCache;

        public Cache(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public static Cache New() => new Cache(new MemoryCache(new MemoryCacheOptions()));

        public void Set<T>(object key, T value, TimeSpan duration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(duration);

            _memoryCache.Set(key, value, cacheEntryOptions);
        }

        public bool TryGet<T>(object key, out T obj) where T : class
        {
            var o = _memoryCache.TryGetValue(key, out T dobj);
            obj = dobj;
            return o;
        }
    }
}
