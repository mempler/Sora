using System;
using System.IO;
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
    [Route("[controller]")]
    [ApiController]
    public class WebController : Controller
    {
        #region GET /web/
        [HttpGet]
        public IActionResult Index()
        {
            return Forbid("ERR: you sneaky little mouse :3");
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

            Scores.InsertScore(db, s.score);
            // TODO: Finish Score Submittion
            return Ok("ok");
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