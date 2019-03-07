using System;
using StackExchange.Redis;

namespace Shared.Helpers
{
    public class Cache
    {
        public Cache()
        {
            Redis = ConnectionMultiplexer.Connect("localhost");
        }
        
        private ConnectionMultiplexer Redis;
        
        public bool CacheString(RedisKey key, string data, int duration = 300) // 5 Minutes.
        {
            return Redis.GetDatabase().StringSet(key, data, TimeSpan.FromSeconds(duration));
        }

        public string GetCachedString(RedisKey key)
        {
            return Redis.GetDatabase().StringGet(key);
        }

        public bool CacheData(RedisKey key, byte[] data, int duration = 300)
        {
            return Redis.GetDatabase().StringSet(key, data, TimeSpan.FromSeconds(duration));
        }

        public byte[] GetCachedData(RedisKey key)
        {
            return Redis.GetDatabase().StringGet(key);
        }
    }
}