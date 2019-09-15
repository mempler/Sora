#region LICENSE

/*
    olSora - A Modular Bancho written in C#
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
