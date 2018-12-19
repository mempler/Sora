using System;
using StackExchange.Redis;

namespace Shared.Helpers
{
    public static class Cache
    {
        private static ConnectionMultiplexer Redis => _actualRedis ?? (_actualRedis = ConnectionMultiplexer.Connect("localhost"));

        private static ConnectionMultiplexer _actualRedis;
        
        public static bool CacheString(RedisKey key, string data, int duration = 300) // 5 Minutes.
        {
            return Redis.GetDatabase().StringSet(key, data, TimeSpan.FromSeconds(duration));
        }

        public static string GetCachedString(RedisKey key)
        {
            return Redis.GetDatabase().StringGet(key);
        }

        public static bool CacheData(RedisKey key, byte[] data, int duration = 300)
        {
            return Redis.GetDatabase().StringSet(key, data, TimeSpan.FromSeconds(duration));
        }

        public static byte[] GetCachedData(RedisKey key)
        {
            return Redis.GetDatabase().StringGet(key);
        }
    }
}