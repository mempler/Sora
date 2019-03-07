using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using EventManager.Attributes;
using EventManager.Enums;
using Jibril.EventArgs;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril.Events
{
    [EventClass]
    public class OnGetReplayEvent
    {
        private readonly Database _db;

        public OnGetReplayEvent(Database db)
        {
            _db = db;
        }

        [Event(EventType.SharedGetReplay)]
        public void OnGetReplay(SharedEventArgs args)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(args.req.Url.Query);
            Users               user  = Users.GetUser(_db, Users.GetUserId(_db, query.Get("u")));
            if (user == null || !user.IsPassword(query.Get("h")))
            {
                args.res.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }

            try
            {
                int    replayId = int.Parse(query.Get("c"));
                Scores score    = Scores.GetScore(_db, replayId);
                using (FileStream s = File.OpenRead("data/replays/" + score.ReplayMd5 + ".rbin"))
                    s.CopyTo(args.res.OutputStream);
            }
            catch
            {
                // Ignored
            }
        }
    }
}