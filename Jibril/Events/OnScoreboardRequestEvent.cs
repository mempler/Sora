using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using EventManager.Attributes;
using EventManager.Enums;
using Jibril.Enums;
using Jibril.EventArgs;
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

        public OnScoreboardRequestEvent(Database db, Cache cache)
        {
            _db = db;
            _cache = cache;
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

            Scoreboard sboard = new Scoreboard(_db,
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