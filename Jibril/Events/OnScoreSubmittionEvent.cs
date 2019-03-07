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
using System.IO;
using System.Text;
using EventManager.Attributes;
using EventManager.Enums;
using HttpMultipartParser;
using Jibril.EventArgs;
using Jibril.Helpers;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril.Events
{
    [EventClass]
    public class OnScoreSubmittionEvent
    {
        private readonly Database _db;

        public OnScoreSubmittionEvent(Database db)
        {
            _db = db;
            if (!Directory.Exists("data/replays"))
                Directory.CreateDirectory("data/replays");
        }
        
        [Event(EventType.SharedScoreSubmittion)]
        public void OnScoreSubmittion(SharedEventArgs args)
        {
             MultipartFormDataParser parser = new MultipartFormDataParser(args.req.InputStream);
            /*
            foreach (ParameterPart parm in parser.Parameters)
                Console.WriteLine($"{parm.Name}: {parm.Data}");
                */

            (bool pass, Scores score) s = 
                ScoreSubmittionParser.ParseScore(_db, parser.GetParameterValue("score"), parser.GetParameterValue("iv"),
                                                 parser.GetParameterValue("osuver"));

            if (s.score.Id == -1)
            {
                args.res.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }
            
            if (!s.score.ScoreOwner.IsPassword(parser.GetParameterValue("pass")))
            {
                args.res.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }

            if (s.score.ScoreOwner == null)
                s.score.ScoreOwner = Users.GetUser(_db, s.score.UserId);

            bool isRelaxing = (s.score.Mods & Mod.Relax) != 0;
            
            if (!s.pass)
            {
                if (isRelaxing)
                {
                    LeaderboardRx rx = LeaderboardRx.GetLeaderboard(_db, s.score.ScoreOwner);
                    rx.IncreasePlaycount(_db, s.score.PlayMode);
                    rx.IncreaseScore(_db, s.score.TotalScore, false, s.score.PlayMode);
                }
                else
                {
                    LeaderboardStd std = LeaderboardStd.GetLeaderboard(_db, s.score.ScoreOwner);
                    std.IncreasePlaycount(_db, s.score.PlayMode);
                    std.IncreaseScore(_db, s.score.TotalScore, false, s.score.PlayMode);
                }
                
                args.res.OutputStream.Write(Encoding.ASCII.GetBytes("Thanks for your hard work!"));
                return;
            }

            if (isRelaxing)
            {
                LeaderboardRx rx = LeaderboardRx.GetLeaderboard(_db, s.score.ScoreOwner);
                rx.IncreasePlaycount(_db, s.score.PlayMode);
                rx.IncreaseCount300(_db, s.score.Count300, s.score.PlayMode);
                rx.IncreaseCount100(_db, s.score.Count100, s.score.PlayMode);
                rx.IncreaseCount50(_db, s.score.Count50, s.score.PlayMode);
                rx.IncreaseScore(_db, s.score.TotalScore, true, s.score.PlayMode);
                rx.IncreaseScore(_db, s.score.TotalScore, false, s.score.PlayMode);
            }
            else
            {
                LeaderboardStd std = LeaderboardStd.GetLeaderboard(_db, s.score.ScoreOwner);
                std.IncreasePlaycount(_db, s.score.PlayMode);
                std.IncreaseCount300(_db, s.score.Count300, s.score.PlayMode);
                std.IncreaseCount100(_db, s.score.Count100, s.score.PlayMode);
                std.IncreaseCount50(_db, s.score.Count50, s.score.PlayMode);
                std.IncreaseScore(_db, s.score.TotalScore, true, s.score.PlayMode);
                std.IncreaseScore(_db, s.score.TotalScore, false, s.score.PlayMode);
            }

            FilePart replay = parser.Files[0];
            using (replay.Data)
            using(MemoryStream stream = new MemoryStream())
            {
                replay.Data.CopyTo(stream);
    
                stream.Position = 0;
    
                if (!Directory.Exists("data/replays"))
                    Directory.CreateDirectory("data/replays");
    
                s.score.ReplayMd5 = Hex.ToHex(Crypto.GetMd5(stream)) ?? string.Empty;
                if (!string.IsNullOrEmpty(s.score.ReplayMd5)) {
                    using (FileStream replayFile = File.Create($"data/replays/{s.score.ReplayMd5}.rbin"))
                    {
                        stream.Position = 0;
                        stream.WriteTo(replayFile);
                        stream.Close();
                        replayFile.Close();
                    }
                }
            }
            
            Scores.InsertScore(_db, s.score);
            
            Console.WriteLine(s.score);
            // TODO: Finish score submittion.
            args.res.OutputStream.Write(Encoding.ASCII.GetBytes("error: Thanks for your hard work!"));
        }
    }
}