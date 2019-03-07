#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

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