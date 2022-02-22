using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace N8T.Infrastructure.Cache
{
    public class MemoryService<T> : ICacheService<T>
    {
        private IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        protected MemoryService(IMemoryCache memoryCache, IConfiguration config, IWebHostEnvironment env)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            int expireSeconds = config.GetValue("Cache:ExpireSeconds", 60 * 10);
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(expireSeconds));
        }

        public virtual T Get(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public virtual bool Set(string key, T data)
        {
            T cached = _memoryCache.Set(key, data, _cacheOptions);
            return cached is not null;
        }

        public virtual bool Remove(string key)
        {
            _memoryCache.Remove(key);
            return true;
        }
    }
}
