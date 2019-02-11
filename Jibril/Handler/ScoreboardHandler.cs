using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using Jibril.Enums;
using Jibril.Objects;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Shared.Models;

namespace Jibril.Handler
{
    public class ScoreboardHandler
    {
        [Handler(HandlerTypes.SharedScoreboardRequest)]
        public void OnScoreboardRequest(HttpListenerRequest request, HttpListenerResponse response,
                                        Dictionary<string, string> args)
        {
            NameValueCollection query   = HttpUtility.ParseQueryString(request.Url.Query);
            string              fileMd5 = query.Get("c");

            Enum.TryParse(query.Get("m"), out PlayMode playmode);
            Enum.TryParse(query.Get("v"), out ScoreboardType scoreboardType);
            //string scoreboardVersion = request.Headers["vv"];
            Enum.TryParse(query.Get("mods"), out Mod mods);

            Users user = Users.GetUser(Users.GetUserId(query.Get("us")));
            if (user == null || !user.IsPassword(query.Get("ha")))
            {
                response.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }

            string cachehash =
                Hex.ToHex(Crypto.GetMd5($"{fileMd5}{playmode}{mods}{scoreboardType}{user.Id}{user.Username}"));

            string cachedData = Cache.GetCachedString($"jibril:Scoreboards:{cachehash}");
            
            if (!string.IsNullOrEmpty(cachedData))
            {
                response.OutputStream.Write(Encoding.UTF8.GetBytes(cachedData));
                return;
            }

            Scoreboard sboard = new Scoreboard(fileMd5, user, playmode,
                                               (mods & Mod.Relax) != 0,
                                               (mods & Mod.TouchDevice) != 0,
                                               scoreboardType == ScoreboardType.Friends,
                                               scoreboardType == ScoreboardType.Country,
                                               scoreboardType == ScoreboardType.Mods,
                                               mods);
                        
            Cache.CacheString($"jibril:Scoreboards:{cachehash}", cachedData = sboard.ToOsuString(), 60);

            response.OutputStream.Write(Encoding.Default.GetBytes(cachedData));
        }
    }
}