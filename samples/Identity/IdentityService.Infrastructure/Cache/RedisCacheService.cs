using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using N8T.Infrastructure.Cache;
using ServiceStack.Redis;

namespace IdentityService.Infrastructure.Cache
{
    public class RedisCacheService<T> : ICacheService<T>
    {
        private readonly IRedisClient _cacheClient;

        public RedisCacheService(IConfiguration config, IWebHostEnvironment env)
        {
            string redisHost = config.GetValue("Cache:ConnectionStrings", "localhost:6379");
            using (RedisManagerPool manager = new RedisManagerPool(redisHost))
            {
                _cacheClient = manager.GetClient();
            }
        }

        public virtual T Get(string key)
        {
            return _cacheClient.Get<T>(key);
        }

        public virtual bool Set(string key, T data)
        {
            return _cacheClient.Set(key, data);
        }

        public virtual bool Remove(string key)
        {
            return _cacheClient.Remove(key);
        }
    }
}
