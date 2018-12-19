using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HttpMultipartParser;
using Jibril.Helpers;
using Shared.Database.Models;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;

namespace Jibril.Handler
{
    public class ScoreSubmittionHandler
    {
        [Handler(HandlerTypes.Initializer)]
        public static void Initializer()
        {
            if (!Directory.Exists("data/replays"))
                Directory.CreateDirectory("data/replays");
        }
        
        [Handler(HandlerTypes.SharedScoreSubmittion)]
        public void OnScoreSubmittion(HttpListenerRequest request, HttpListenerResponse response,
                                        Dictionary<string, string> args)
        {
            MultipartFormDataParser parser = new MultipartFormDataParser(request.InputStream);
            /*
            foreach (ParameterPart parm in parser.Parameters)
                Console.WriteLine($"{parm.Name}: {parm.Data}");
                */

            (bool pass, Scores score) s = 
                ScoreSubmittionParser.ParseScore(parser.GetParameterValue("score"), parser.GetParameterValue("iv"),
                                                 parser.GetParameterValue("osuver"));

            if (s.score.Id == -1)
            {
                response.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }
            
            if (!s.score.ScoreOwner.IsPassword(parser.GetParameterValue("pass")))
            {
                response.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }

            if (s.score.ScoreOwner == null)
                s.score.ScoreOwner = Users.GetUser(s.score.UserId);


            bool isTouching = (s.score.Mods & Mod.TouchDevice) != 0;
            bool isRelaxing = (s.score.Mods & Mod.Relax) != 0;
            
            if (!s.pass)
            {
                if (isTouching)
                {
                    LeaderboardTouch touch = LeaderboardTouch.GetLeaderboard(s.score.ScoreOwner);
                    touch.IncreasePlaycount(s.score.PlayMode);
                    touch.IncreaseScore(s.score.TotalScore, false, s.score.PlayMode);
                } else if (isRelaxing)
                {
                    LeaderboardTouch rx = LeaderboardTouch.GetLeaderboard(s.score.ScoreOwner);
                    rx.IncreasePlaycount(s.score.PlayMode);
                    rx.IncreaseScore(s.score.TotalScore, false, s.score.PlayMode);
                }
                else
                {
                    LeaderboardTouch std = LeaderboardTouch.GetLeaderboard(s.score.ScoreOwner);
                    std.IncreasePlaycount(s.score.PlayMode);
                    std.IncreaseScore(s.score.TotalScore, false, s.score.PlayMode);
                }
                
                response.OutputStream.Write(Encoding.ASCII.GetBytes("Thanks for your hard work!"));
                return;
            }

            if (isTouching)
            {
                LeaderboardTouch touch = LeaderboardTouch.GetLeaderboard(s.score.ScoreOwner);
                touch.IncreaseCount300(s.score.Count300, s.score.PlayMode);
                touch.IncreaseCount100(s.score.Count100, s.score.PlayMode);
                touch.IncreaseCount50(s.score.Count50, s.score.PlayMode);
                touch.IncreaseCountMiss(s.score.CountMiss, s.score.PlayMode);
                touch.IncreasePlaycount(s.score.PlayMode);
                touch.IncreaseScore(s.score.TotalScore, true, s.score.PlayMode);
                touch.IncreaseScore(s.score.TotalScore, false, s.score.PlayMode);
            }
            else if (isRelaxing)
            {
                LeaderboardTouch rx = LeaderboardTouch.GetLeaderboard(s.score.ScoreOwner);
                rx.IncreasePlaycount(s.score.PlayMode);
                rx.IncreaseCount300(s.score.Count300, s.score.PlayMode);
                rx.IncreaseCount100(s.score.Count100, s.score.PlayMode);
                rx.IncreaseCount50(s.score.Count50, s.score.PlayMode);
                rx.IncreaseScore(s.score.TotalScore, true, s.score.PlayMode);
                rx.IncreaseScore(s.score.TotalScore, false, s.score.PlayMode);
            }
            else
            {
                LeaderboardTouch std = LeaderboardTouch.GetLeaderboard(s.score.ScoreOwner);
                std.IncreasePlaycount(s.score.PlayMode);
                std.IncreaseCount300(s.score.Count300, s.score.PlayMode);
                std.IncreaseCount100(s.score.Count100, s.score.PlayMode);
                std.IncreaseCount50(s.score.Count50, s.score.PlayMode);
                std.IncreaseScore(s.score.TotalScore, true, s.score.PlayMode);
                std.IncreaseScore(s.score.TotalScore, false, s.score.PlayMode);
            }

            FilePart replay = parser.Files[0];
            using (replay.Data)
            using(MemoryStream stream = new MemoryStream())
            {
                replay.Data.CopyTo(stream);
    
                stream.Position = 0;
    
                if (!Directory.Exists("data/replays"))
                    Directory.CreateDirectory("data/replays");
    
                s.score.ReplayMD5 = Hex.ToHex(Crypto.GetMd5(stream)) ?? string.Empty;
                if (!string.IsNullOrEmpty(s.score.ReplayMD5)) {
                    using (FileStream replayFile = File.Create($"data/replays/{s.score.FileMD5}.rbin"))
                    {
                        stream.Position = 0;
                        stream.WriteTo(replayFile);
                        stream.Close();
                        replayFile.Close();
                    }
                }
            }
            
            Scores.InsertScore(s.score);
            
            Console.WriteLine(s.score);
            // TODO: Finish score submittion.
            response.OutputStream.Write(Encoding.ASCII.GetBytes("error: Thanks for your hard work!"));
        }
    }
}