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

using System.IO;
using System.Text;
using EventManager.Attributes;
using EventManager.Enums;
using Jibril.EventArgs;
using Shared.Helpers;

namespace Jibril.Events
{
    [EventClass]
    public class OnAvatarsEvent
    {
        private readonly Cache _cache;

        public OnAvatarsEvent(Cache cache)
        {
            _cache = cache;
            
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");
        }
        
        [Event(EventType.SharedAvatars)]
        public void OnAvatars(SharedEventArgs args)
        {
            args.args.TryGetValue("avatar", out string avi);
            int.TryParse(avi ?? "0", out int avatarId);

            byte[] cachedData = _cache.GetCachedData($"jibril:avatars:{avatarId.ToString()}");
            if (cachedData != null && cachedData.Length > 0)
            {
                args.res.OutputStream.Write(cachedData);
                return;
            }

            byte[] data;

            if (avatarId == 0 || !File.Exists($"data/avatars/{avatarId.ToString()}"))
            {
                if (File.Exists("data/avatars/default"))
                    data = File.ReadAllBytes("data/avatars/default");
                else
                {
                    args.res.OutputStream.Write(Encoding.ASCII.GetBytes(
                                                    "Sorry to tell you, but there is no default avatar!\n" +
                                                    "if you're the owner of this instance please set an default " +
                                                    "by adding a png/jpg file called default"));
                    return;
                }
            }
            else
                data = File.ReadAllBytes($"data/avatars/{avatarId.ToString()}");

            _cache.CacheData($"jibril:avatars:{avatarId.ToString()}", data);

            args.res.OutputStream.Write(data);
        }
    }
}