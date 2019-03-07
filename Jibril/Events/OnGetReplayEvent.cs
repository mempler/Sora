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