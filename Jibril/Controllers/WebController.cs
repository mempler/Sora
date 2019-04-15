using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jibril.Enums;
using Jibril.Helpers;
using Jibril.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeppyPoints;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebController : Controller
    {
        #region GET /web/
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("ERR: you sneaky little mouse :3");
        }
        #endregion
        
        #region GET /web/osu-osz2-getscores.php
        [HttpGet("osu-osz2-getscores.php")]
        public IActionResult GetScoreResult(
            [FromServices] Database db,
            [FromServices] Cache cache,
            [FromServices] Config conf,
            
            [FromQuery(Name = "s")] int s,
            [FromQuery(Name = "vv")] int scoreboardVersion,
            [FromQuery(Name = "v")] ScoreboardType type,
            [FromQuery(Name = "c")] string fileMD5,
            [FromQuery(Name = "f")] string f,
            [FromQuery(Name = "m")] PlayMode playMode,
            [FromQuery(Name = "i")] int i,
            [FromQuery(Name = "mods")] Mod mods,
            [FromQuery(Name = "h")] string h,
            [FromQuery(Name = "aa")] int a,
            [FromQuery(Name = "us")] string us,
            [FromQuery(Name = "ha")] string pa)
        {

            Users user = Users.GetUser(db, Users.GetUserId(db, us));
            if (user == null || !user.IsPassword(pa))
                return Ok("error: pass");

            string cache_hash =
                Hex.ToHex(Crypto.GetMd5(
                              $"{fileMD5}{playMode}{mods}{type}{user.Id}{user.Username}"));

            string cachedData = cache.GetCachedString($"jibril:Scoreboards:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Scoreboard sboard = new Scoreboard(db, conf,
                                               fileMD5, user, playMode,
                                               (mods & Mod.Relax) != 0,
                                               type == ScoreboardType.Friends,
                                               type == ScoreboardType.Country,
                                               type == ScoreboardType.Mods,
                                               mods);

            cache.CacheString($"jibril:Scoreboards:{cache_hash}", cachedData = sboard.ToOsuString(db), 1);         
            
            return Ok(cachedData);
        }
        #endregion

        #region POST /web/osu-submit-modular-selector.php

        [HttpPost("osu-submit-modular-selector.php")]
        public IActionResult PostSubmitModular(
            [FromServices] Database db,
            [FromServices] Config cfg
            )
        {
            string score = Request.Form["score"];
            string iv = Request.Form["iv"];
            string osuver = Request.Form["osuver"];
            string pass = Request.Form["pass"];
            
            (bool pass, Scores score) s =
                ScoreSubmittionParser.ParseScore(db, score, iv,osuver);

            if (s.score.UserId == -1)
                return Ok("error: pass");

            if (s.score.ScoreOwner == null)
                s.score.ScoreOwner = Users.GetUser(db, s.score.UserId);
            
            if (s.score.ScoreOwner == null)
                return Ok("error: pass");
            
            if (!s.score.ScoreOwner.IsPassword(pass))
                return Ok("error: pass");

            bool isRelaxing = (s.score.Mods & Mod.Relax) != 0;
            
             if (!s.pass)
            {
                if (isRelaxing)
                {
                    LeaderboardRx rx = LeaderboardRx.GetLeaderboard(db, s.score.ScoreOwner);
                    rx.IncreasePlaycount(db, s.score.PlayMode);
                    rx.IncreaseScore(db, s.score.TotalScore, false, s.score.PlayMode);
                }
                else
                {
                    LeaderboardStd std = LeaderboardStd.GetLeaderboard(db, s.score.ScoreOwner);
                    std.IncreasePlaycount(db, s.score.PlayMode);
                    std.IncreaseScore(db, s.score.TotalScore, false, s.score.PlayMode);
                }
                
                return Ok("Thanks for your hard work!");
            }

             switch (s.score.PlayMode)
             {
                 case PlayMode.Osu:
                     oppai op = new oppai(BeatmapDownloader.GetBeatmap(s.score.FileMd5, cfg));
                     op.SetAcc((int) s.score.Count300, (int) s.score.Count50, (int) s.score.CountMiss);
                     op.SetCombo(s.score.MaxCombo);
                     op.SetMods(s.score.Mods);                    
                     op.Calculate();
                     
                     s.score.PeppyPoints = op.GetPP();
                     Console.WriteLine(s.score.PeppyPoints);
                     break;
             }

            IFormFile ReplayFile = Request.Form.Files.GetFile("score");

            if (!Directory.Exists("data/replays"))
                Directory.CreateDirectory("data/replays");

            using (MemoryStream m = new MemoryStream())
            {
                ReplayFile.CopyTo(m);

                s.score.ReplayMd5 = Hex.ToHex(Crypto.GetMd5(m)) ?? string.Empty;
                if (!string.IsNullOrEmpty(s.score.ReplayMd5))
                  using (FileStream replayFile = System.IO.File.Create($"data/replays/{s.score.ReplayMd5}"))
                  {
                      m.Position = 0;
                      m.WriteTo(replayFile);
                      m.Close();
                      replayFile.Close();
                  }
            }

            Scores oldScore = Scores.GetScores(db,
                                               s.score.FileMd5,
                                               s.score.ScoreOwner,
                                               s.score.PlayMode,
                                               isRelaxing,
                                               false,
                                               false,
                                               false,
                                               s.score.Mods,
                                        true).FirstOrDefault();
            
            LeaderboardRx oldRx = LeaderboardRx.GetLeaderboard(db, s.score.ScoreOwner);
            LeaderboardStd oldStd = LeaderboardStd.GetLeaderboard(db, s.score.ScoreOwner);

            Scores.InsertScore(db, s.score);

            if (isRelaxing)
            {
                LeaderboardRx rx = LeaderboardRx.GetLeaderboard(db, s.score.ScoreOwner);
                rx.IncreasePlaycount(db, s.score.PlayMode);
                rx.IncreaseCount300(db, s.score.Count300, s.score.PlayMode);
                rx.IncreaseCount100(db, s.score.Count100, s.score.PlayMode);
                rx.IncreaseCount50(db, s.score.Count50, s.score.PlayMode);
                rx.IncreaseScore(db, s.score.TotalScore, true, s.score.PlayMode);
                rx.IncreaseScore(db, s.score.TotalScore, false, s.score.PlayMode);

                rx.UpdatePP(db, s.score.PlayMode);
            }
            else
            {
                LeaderboardStd std = LeaderboardStd.GetLeaderboard(db, s.score.ScoreOwner);
                std.IncreasePlaycount(db, s.score.PlayMode);
                std.IncreaseCount300(db, s.score.Count300, s.score.PlayMode);
                std.IncreaseCount100(db, s.score.Count100, s.score.PlayMode);
                std.IncreaseCount50(db, s.score.Count50, s.score.PlayMode);
                std.IncreaseScore(db, s.score.TotalScore, true, s.score.PlayMode);
                std.IncreaseScore(db, s.score.TotalScore, false, s.score.PlayMode);

                std.UpdatePP(db, s.score.PlayMode);
            }


            Scores NewScore = Scores.GetScores(db,
                                               s.score.FileMd5,
                                               s.score.ScoreOwner,
                                               s.score.PlayMode,
                                               isRelaxing,
                                               false,
                                               false,
                                               false,
                                               s.score.Mods,
                                               true).FirstOrDefault();
            
            Cheesegull cg = new Cheesegull(cfg);
            try
            {
                cg.SetBM(s.score.FileMd5);
            }
            catch
            {
                // ignored
            }

            CheesegullBeatmap bm;
            List<CheesegullBeatmapSet> sets = cg.GetSets();
            if (sets == null)
            {
                bm = new CheesegullBeatmap();
            }
            else
            {
                bm = sets[0].ChildrenBeatmaps.First(x => x.FileMD5 == s.score.FileMd5);
            }
            
            Chart bmChart = new Chart(
                "beatmap",
                "Beatmap Ranking",
                $"https://osu.ppy.sh/b/{bm.BeatmapID}",
                oldScore?.Position ?? 0,
                NewScore?.Position ?? 0,
                oldScore?.MaxCombo ?? 0,
                NewScore?.MaxCombo ?? 0,
                oldScore?.Accuracy ?? 0,
                NewScore?.Accuracy ?? 0,
                oldScore?.TotalScore ?? 0,
                NewScore?.TotalScore ?? 0,
                oldScore?.PeppyPoints ?? 0,
                NewScore?.PeppyPoints ?? 0,
                NewScore?.Id ?? 0
                );

            /*
            Chart overallChart = new Chart(
                "overall",
                "Overall Ranking",
                $"https://osu.ppy.sh/u/{s.score.ScoreOwner.Id}",
                isRelaxing ? oldRx.P
                NewScore?.Position ?? 0,
                oldScore?.MaxCombo ?? 0,
                NewScore?.MaxCombo ?? 0,
                oldScore?.Accuracy ?? 0,
                NewScore?.Accuracy ?? 0,
                oldScore?.TotalScore ?? 0,
                NewScore?.TotalScore ?? 0,
                oldScore?.PeppyPoints ?? 0,
                NewScore?.PeppyPoints ?? 0,
                NewScore?.Id ?? 0
            );
            */
            
            /*
            beatmapId:315|beatmapSetId:141|beatmapPlaycount:1|beatmapPasscount:1|approvedDate:

            chartId:overall|
            chartUrl:https://akatsuki.pw/u/1001|
            chartName:Overall Ranking|
            rankBefore:0|
            rankAfter:1|
            rankedScoreBefore:554957857|
            rankedScoreAfter:555075201|
            totalScoreBefore:2473528962|
            totalScoreAfter:2473646306|
            maxComboBefore:2219|
            maxComboAfter:2219|
            accuracyBefore:96.985397338867|
            accuracyAfter:96.985397338867|
            ppBefore:7347|
            ppAfter:7347|
            achievements-new:|
            onlineScoreId:500335289   
            */
            return Ok();
        }
        
        #endregion

        #region GET /web/osu-getreplay.php

        [HttpGet("osu-getreplay.php")]
        public IActionResult GetReplay(
            [FromServices] Database db,
            
            [FromQuery(Name = "c")] int replayId,
            [FromQuery(Name = "m")] PlayMode mode,
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
            )
        {
            Users user = Users.GetUser(db, Users.GetUserId(db, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            Scores s = Scores.GetScore(db, replayId);
            if (s == null)
                return NotFound();

            return File(System.IO.File.OpenRead("data/replays/" + s.ReplayMd5), "binary/octet-stream", s.ReplayMd5);
        }

        #endregion

        #region GET /web/osu-search.php

        [HttpGet("osu-search.php")]
        public IActionResult GetSearchDIRECT(
            [FromServices] Database db,
            [FromServices] Cache cache,
            [FromServices] Config cfg,

            [FromQuery(Name = "m")] int playMode,
            [FromQuery(Name = "r")] int rankedStatus,
            [FromQuery(Name = "p")] int page,
            [FromQuery(Name = "q")] string query,
            
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            Users user = Users.GetUser(db, Users.GetUserId(db, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            string cache_hash = Hex.ToHex(Crypto.GetMd5($"m{playMode}r{rankedStatus}p{page}q{query}"));

            string cachedData = cache.GetCachedString($"jibril:DirectSearches:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Cheesegull cg = new Cheesegull(cfg);
            try
            {
                cg.Search(query, rankedStatus, playMode, page);
            }
            catch
            {
                // Ignored
            }

            Response.ContentType = "text/plain";

            cache.CacheString($"jibril:DirectSearches:{cache_hash}", cachedData = cg.ToDirect(), 600);
            
            return Ok(cachedData);
        }

        #endregion

        #region GET /web/osu-search-set.php
        [HttpGet("osu-search-set.php")]
        public IActionResult GetDirectNP(
            [FromServices] Database db,
            [FromServices] Cache cache,
            [FromServices] Config cfg,

            [FromQuery(Name = "s")] int setId,
            [FromQuery(Name = "b")] int beatmapId,

            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            Users user = Users.GetUser(db, Users.GetUserId(db, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            string cache_hash = Hex.ToHex(Crypto.GetMd5($"s{setId}"));

            string cachedData = cache.GetCachedString($"jibril:DirectNP:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Cheesegull cg = new Cheesegull(cfg);
            try
            {
                if (setId != 0)
                    cg.SetBMSet(setId);
                else
                    cg.SetBM(beatmapId);
            }
            catch
            {
                // ignored
            }

            Response.ContentType = "text/plain";

            cache.CacheString($"jibril:DirectNP:{cache_hash}", cachedData = cg.ToNP(), 600);

            return Ok(cachedData);
        }
        #endregion
    }
}