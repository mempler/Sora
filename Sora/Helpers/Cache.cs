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
using System.Runtime.InteropServices;
using StackExchange.Redis;

namespace Sora.Helpers
{
    public class Cache
    {
        public Cache(IConfig config)
        {
            Redis = ConnectionMultiplexer.Connect(config.Redis.Hostname);
            if (!Redis.IsConnected)
                throw new Exception("Failed to Connect to Redis!");
        }
        
        private readonly ConnectionMultiplexer Redis;
        
        public bool CacheString(RedisKey key, string data, int duration = 300) // 5 Minutes.
        {
            return Redis.GetDatabase().StringSet(key, data, TimeSpan.FromSeconds(duration));
        }

        public string GetCachedString(RedisKey key)
        {
            return Redis.GetDatabase().StringGet(key);
        }
        
        public void CacheStruct<T>(RedisKey key, ref T data, int duration = 1800) where T : struct
        {
            int    size   = Marshal.SizeOf(data);
            IntPtr arrPtr = Marshal.AllocHGlobal(size);
            
            Marshal.StructureToPtr(data, arrPtr, true);
            byte[] arr = new byte[size];
            Marshal.Copy(arrPtr, arr, 0, size);
            Marshal.FreeHGlobal(arrPtr);

            CacheData(key, arr);
        }

        public T GetCachedStruct<T>(RedisKey key) where T : struct
        {
            byte[] arr = GetCachedData(key);
            if (arr == null)
                return default;
            
            T structure = new T();

            int    size = Marshal.SizeOf<T>();
            IntPtr ptr  = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            structure = (T) Marshal.PtrToStructure(ptr, structure.GetType());
            Marshal.FreeHGlobal(ptr);

            return structure;
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