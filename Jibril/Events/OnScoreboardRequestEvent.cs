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
using System.Collections.Specialized;
using System.Text;
using System.Web;
using EventManager.Attributes;
using EventManager.Enums;
using Jibril.Enums;
using Jibril.EventArgs;
using Jibril.Helpers;
using Jibril.Objects;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril.Events
{
    [EventClass]
    public class OnScoreboardRequestEvent
    {
        private readonly Database _db;
        private readonly Cache _cache;
        private readonly Config _cfg;

        public OnScoreboardRequestEvent(Database db, Cache cache, Config cfg)
        {
            _db = db;
            _cache = cache;
            _cfg = cfg;
        }
        
        [Event(EventType.SharedScoreboardRequest)]
        public void OnScoreboardRequest(SharedEventArgs args)
        {
            NameValueCollection query   = HttpUtility.ParseQueryString(args.req.Url.Query);
            string              fileMd5 = query.Get("c");

            Enum.TryParse(query.Get("m"), out PlayMode playmode);
            Enum.TryParse(query.Get("v"), out ScoreboardType scoreboardType);
            //string scoreboardVersion = request.Headers["vv"];
            Enum.TryParse(query.Get("mods"), out Mod mods);

            Users user = Users.GetUser(_db, Users.GetUserId(_db, query.Get("us")));
            if (user == null || !user.IsPassword(query.Get("ha")))
            {
                args.res.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }

            string cache_hash =
                Hex.ToHex(Crypto.GetMd5($"{fileMd5}{playmode}{mods}{scoreboardType}{user.Id}{user.Username}"));

            string cachedData = _cache.GetCachedString($"jibril:Scoreboards:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
            {
                args.res.OutputStream.Write(Encoding.UTF8.GetBytes(cachedData));
                return;
            }

            Scoreboard sboard = new Scoreboard(_db, _cfg,
                                               fileMd5, user, playmode,
                                               (mods & Mod.Relax) != 0,
                                               (mods & Mod.TouchDevice) != 0,
                                               scoreboardType == ScoreboardType.Friends,
                                               scoreboardType == ScoreboardType.Country,
                                               scoreboardType == ScoreboardType.Mods,
                                               mods);
            
            _cache.CacheString($"jibril:Scoreboards:{cache_hash}", cachedData = sboard.ToOsuString(_db), 60);

            args.res.OutputStream.Write(Encoding.Default.GetBytes(cachedData));
        }
    }
}