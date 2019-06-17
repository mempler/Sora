using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Helpers;
using Sora.Objects;
using Sora.Services;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/web/")]
    public class WebController : Controller
    {
        private readonly SoraDbContextFactory _factory;
        private readonly EventManager _ev;
        private readonly Cache _cache;
        private readonly Config _config;
        private readonly Bot.Sora _sora;
        private readonly PerformancePointsProcessor _pointsProcessor;
        private readonly PresenceService _ps;

        public WebController(SoraDbContextFactory factory,
                             EventManager ev,
                             Cache cache,
                             Config config,
                             Bot.Sora sora,
                             PerformancePointsProcessor pointsProcessor,
                             PresenceService ps)
        {
            _factory = factory;
            _ev = ev;
            _cache = cache;
            _config = config;
            _sora = sora;
            _pointsProcessor = pointsProcessor;
            _ps = ps;
        }
        
        #region GET /web/
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("ERR: you sneaky little mouse :3");
        }
        #endregion

        #region GET /web/osu-osz2-getscores.php
        [HttpGet("osu-osz2-getscores.php")]
        public IActionResult GetScoreResult (
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
            Users user = Users.GetUser(_factory, Users.GetUserId(_factory, us));
            if (user == null || !user.IsPassword(pa))
                return Ok("error: pass");

            string cache_hash =
                Hex.ToHex(Crypto.GetMd5(
                              $"{fileMD5}{playMode}{mods}{type}{user.Id}{user.Username}"));

            string cachedData = _cache.GetCachedString($"sora:Scoreboards:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Scoreboard sboard = new Scoreboard(_factory, _config,
                                               fileMD5, user, playMode,
                                               (mods & Mod.Relax) != 0,
                                               type == ScoreboardType.Friends,
                                               type == ScoreboardType.Country,
                                               type == ScoreboardType.Mods,
                                               mods);

            _cache.CacheString($"sora:Scoreboards:{cache_hash}", cachedData = sboard.ToOsuString(), 30);         
            
            return Ok(cachedData);
        }
        #endregion

        #region POST /web/osu-submit-modular-selector.php

        [HttpPost("osu-submit-modular-selector.php")]
        public async Task<IActionResult> PostSubmitModular()
        {
            string score = Request.Form["score"];
            string iv = Request.Form["iv"];
            string osuver = Request.Form["osuver"];
            string pass = Request.Form["pass"];
            
            (bool b, Scores scores) = ScoreSubmittionParser.ParseScore(_factory, score, iv,osuver);

            if (scores.UserId == -1)
                return Ok("error: pass");

            if (scores.ScoreOwner == null)
                scores.ScoreOwner = Users.GetUser(_factory, scores.UserId);
            
            if (scores.ScoreOwner == null)
                return Ok("error: pass");
            
            if (!scores.ScoreOwner.IsPassword(pass))
                return Ok("error: pass");

            bool isRelaxing = (scores.Mods & Mod.Relax) != 0;
            Presence pr = _ps.GetPresence(scores.ScoreOwner.Id);
            
            if (!b || !RankedMods.IsRanked(scores.Mods))
            {
                if (isRelaxing)
                {
                    LeaderboardRx rx = LeaderboardRx.GetLeaderboard(_factory, scores.ScoreOwner);
                    rx.IncreasePlaycount(_factory, scores.PlayMode);
                    rx.IncreaseScore(_factory, scores.TotalScore, false, scores.PlayMode);
                }
                else
                {
                    LeaderboardStd std = LeaderboardStd.GetLeaderboard(_factory, scores.ScoreOwner);
                    std.IncreasePlaycount(_factory, scores.PlayMode);
                    std.IncreaseScore(_factory, scores.TotalScore, false, scores.PlayMode);
                }

                await _ev.RunEvent(EventType.BanchoUserStatsRequest, new BanchoUserStatsRequestArgs
                {
                    userIds = new List<int> {scores.ScoreOwner.Id},
                    pr      = pr
                });

                return Ok("Thanks for your hard work!");
            }

            /*
            switch (scores.PlayMode)
            {
                case PlayMode.Osu:
                    oppai op = new oppai(BeatmapDownloader.GetBeatmap(scores.FileMd5, _config));
                    op.SetAcc((int) scores.Count300, (int) scores.Count50, (int) scores.CountMiss);
                    op.SetCombo(scores.MaxCombo);
                    op.SetMods(scores.Mods);
                    op.Calculate();
                     
                    scores.PeppyPoints = op.GetPP();
                    Logger.Info("Peppy Points:", scores.PeppyPoints);
                    break;
            }
            */

            IFormFile ReplayFile = Request.Form.Files.GetFile("score");

            if (!Directory.Exists("data/replays"))
                Directory.CreateDirectory("data/replays");

            await using (MemoryStream m = new MemoryStream())
            {
                ReplayFile.CopyTo(m);
                m.Position = 0;
                scores.ReplayMd5 = Hex.ToHex(Crypto.GetMd5(m)) ?? string.Empty;
                if (!string.IsNullOrEmpty(scores.ReplayMd5)) 
                    await using (FileStream replayFile = System.IO.File.Create($"data/replays/{scores.ReplayMd5}"))
                    {
                        m.Position = 0;
                        m.WriteTo(replayFile);
                        m.Close();
                        replayFile.Close();
                    }
            }

            BeatmapDownloader.GetBeatmap(scores.FileMd5, _config);

            if (isRelaxing)
                scores.Mods -= Mod.Relax;
            scores.PeppyPoints =  _pointsProcessor.Compute(scores);
            if (isRelaxing)
                scores.Mods |= Mod.Relax;
            
            Scores oldScore = Scores.GetScores(_factory,
                                               scores.FileMd5,
                                               scores.ScoreOwner,
                                               scores.PlayMode,
                                               isRelaxing,
                                               false,
                                               false,
                                               false,
                                               scores.Mods,
                                        true).FirstOrDefault();
            
            LeaderboardStd oldStd    = LeaderboardStd.GetLeaderboard(_factory, scores.ScoreOwner);
            uint           oldStdPos = oldStd.GetPosition(_factory, scores.PlayMode);

            if (oldScore != null && oldScore.TotalScore <= scores.TotalScore)
            {
                using DatabaseWriteUsage db = _factory.GetForWrite();
                db.Context.Scores.Remove(oldScore);
                System.IO.File.Delete($"data/replays/{oldScore.ReplayMd5}");

                Scores.InsertScore(_factory, scores);
            } else if (oldScore == null)
                Scores.InsertScore(_factory, scores);
            else
                System.IO.File.Delete($"data/replays/{scores.ReplayMd5}");

            if (isRelaxing)
            {
                LeaderboardRx rx = LeaderboardRx.GetLeaderboard(_factory, scores.ScoreOwner);
                rx.IncreasePlaycount(_factory, scores.PlayMode);
                rx.IncreaseCount300(_factory, scores.Count300, scores.PlayMode);
                rx.IncreaseCount100(_factory, scores.Count100, scores.PlayMode);
                rx.IncreaseCount50(_factory, scores.Count50, scores.PlayMode);
                rx.IncreaseScore(_factory, scores.TotalScore, true, scores.PlayMode);
                rx.IncreaseScore(_factory, scores.TotalScore, false, scores.PlayMode);

                rx.UpdatePP(_factory, scores.PlayMode);
                
                pr.LeaderboardRx = rx;
                await _ev.RunEvent(EventType.BanchoUserStatsRequest, new BanchoUserStatsRequestArgs
                {
                    userIds = new List<int> {scores.ScoreOwner.Id},
                    pr      = pr
                });
            }
            else
            {
                LeaderboardStd std = LeaderboardStd.GetLeaderboard(_factory, scores.ScoreOwner);
                std.IncreasePlaycount(_factory, scores.PlayMode);
                std.IncreaseCount300(_factory, scores.Count300, scores.PlayMode);
                std.IncreaseCount100(_factory, scores.Count100, scores.PlayMode);
                std.IncreaseCount50(_factory, scores.Count50, scores.PlayMode);
                std.IncreaseScore(_factory, scores.TotalScore, true, scores.PlayMode);
                std.IncreaseScore(_factory, scores.TotalScore, false, scores.PlayMode);

                std.UpdatePP(_factory, scores.PlayMode);
            }
            
            LeaderboardStd newStd    = LeaderboardStd.GetLeaderboard(_factory, scores.ScoreOwner);
            uint           newStdPos = newStd.GetPosition(_factory, scores.PlayMode);


            Scores NewScore = Scores.GetScores(_factory,
                                               scores.FileMd5,
                                               scores.ScoreOwner,
                                               scores.PlayMode,
                                               isRelaxing,
                                               false,
                                               false,
                                               false,
                                               scores.Mods,
                                               true).FirstOrDefault();
            
            Cheesegull cg = new Cheesegull(_config);
            cg.SetBM(scores.FileMd5);

            List<CheesegullBeatmapSet> sets = cg.GetSets();
            CheesegullBeatmap          bm   = sets?[0].ChildrenBeatmaps.First(x => x.FileMD5 == scores.FileMd5) ?? new CheesegullBeatmap();
            
            double oldAcc;
            double newAcc;

            ulong oldRankedScore;
            ulong newRankedScore;

            double oldPP;
            double newPP;
            
            switch (scores.PlayMode)
            {
                case PlayMode.Osu:
                    oldAcc = Accuracy.GetAccuracy(oldStd.Count300Osu,
                                                  oldStd.Count100Osu,
                                                  oldStd.Count50Osu,
                                                  oldStd.Count300Osu, 0, 0,
                                                  PlayMode.Osu);

                    newAcc = Accuracy.GetAccuracy(newStd.Count300Osu,
                                                  newStd.Count100Osu,
                                                  newStd.Count50Osu,
                                                  newStd.Count300Osu, 0, 0,
                                                  PlayMode.Osu);

                    oldRankedScore = oldStd.RankedScoreOsu;
                    newRankedScore = newStd.RankedScoreOsu;
                    
                    oldPP = oldStd.PerformancePointsOsu;
                    newPP = newStd.PerformancePointsOsu;
                    break;
                case PlayMode.Taiko:
                    oldAcc = Accuracy.GetAccuracy(oldStd.Count300Taiko,
                                                  oldStd.Count100Taiko,
                                                  oldStd.Count50Taiko,
                                                  oldStd.Count300Taiko, 0, 0,
                                                  PlayMode.Taiko);

                    newAcc = Accuracy.GetAccuracy(newStd.Count300Taiko,
                                                  newStd.Count100Taiko,
                                                  newStd.Count50Taiko,
                                                  newStd.Count300Taiko, 0, 0,
                                                  PlayMode.Taiko);

                    oldRankedScore = oldStd.RankedScoreTaiko;
                    newRankedScore = newStd.RankedScoreTaiko;

                    oldPP = oldStd.PerformancePointsTaiko;
                    newPP = newStd.PerformancePointsTaiko;
                    break;
                case PlayMode.Ctb:
                    oldAcc = Accuracy.GetAccuracy(oldStd.Count300Ctb,
                                                  oldStd.Count100Ctb,
                                                  oldStd.Count50Ctb,
                                                  oldStd.Count300Ctb, 0, 0,
                                                  PlayMode.Ctb);

                    newAcc = Accuracy.GetAccuracy(newStd.Count300Ctb,
                                                  newStd.Count100Ctb,
                                                  newStd.Count50Ctb,
                                                  newStd.Count300Ctb, 0, 0,
                                                  PlayMode.Ctb);

                    oldRankedScore = oldStd.RankedScoreCtb;
                    newRankedScore = newStd.RankedScoreCtb;

                    oldPP = oldStd.PerformancePointsCtb;
                    newPP = newStd.PerformancePointsCtb;
                    break;
                case PlayMode.Mania:
                    oldAcc = Accuracy.GetAccuracy(oldStd.Count300Mania,
                                                  oldStd.Count100Mania,
                                                  oldStd.Count50Mania,
                                                  oldStd.Count300Mania, 0, 0,
                                                  PlayMode.Mania);

                    newAcc = Accuracy.GetAccuracy(newStd.Count300Mania,
                                                  newStd.Count100Mania,
                                                  newStd.Count50Mania,
                                                  newStd.Count300Mania, 0, 0,
                                                  PlayMode.Mania);
                    
                    oldRankedScore = oldStd.RankedScoreMania;
                    newRankedScore = newStd.RankedScoreMania;

                    oldPP = oldStd.PerformancePointsMania;
                    newPP = newStd.PerformancePointsMania;
                    break;
                default:
                    return Ok("");
            }
            
            if (NewScore?.Position == 1 && (oldScore == null || oldScore.TotalScore < NewScore.TotalScore))
                _sora.SendMessage(
                    $"[http://{_config.Server.Hostname}/{scores.ScoreOwner.Id} {scores.ScoreOwner.Username}] " +
                    $"has reached #1 on [https://osu.ppy.sh/b/{bm.BeatmapID} {sets?[0].Title} [{bm.DiffName}]] " +
                    $"using {ModUtil.ToString(NewScore.Mods)} " +
                    $"Good job! +{NewScore.PeppyPoints:F}PP",
                    "#announce",
                    false);
            
            Logger.Info(
                $"{L_COL.RED}{scores?.ScoreOwner.Username}",
                        $"{L_COL.PURPLE}( {scores?.ScoreOwner.Id} ){L_COL.WHITE}",
                        $"has just submitted a Score! he earned {L_COL.BLUE}{NewScore?.PeppyPoints:F}PP",
                        $"{L_COL.WHITE}with an Accuracy of {L_COL.RED}{NewScore?.Accuracy * 100:F}",
                        $"{L_COL.WHITE}on {L_COL.YELLOW}{sets?[0].Title} [{bm.DiffName}]",
                        $"{L_COL.WHITE}using {L_COL.BLUE}{ModUtil.ToString(NewScore?.Mods ?? Mod.None)}");

            Chart bmChart = new Chart(
                "beatmap",
                "Beatmap Ranking",
                $"https://osu.ppy.sh/b/{bm.BeatmapID}",
                oldScore?.Position ?? 0,
                NewScore?.Position ?? 0,
                oldScore?.MaxCombo ?? 0,
                NewScore?.MaxCombo ?? 0,
                oldScore?.Accuracy * 100 ?? 0,
                NewScore?.Accuracy * 100 ?? 0,
                oldScore?.TotalScore ?? 0,
                NewScore?.TotalScore ?? 0,
                oldScore?.PeppyPoints ?? 0,
                NewScore?.PeppyPoints ?? 0,
                NewScore?.Id ?? 0
            );

            cg.SetBMSet(bm.ParentSetID);
            
            Chart overallChart = new Chart(
                "overall",
                "Global Ranking",
                $"https://osu.ppy.sh/u/{scores.ScoreOwner.Id}",
                (int) oldStdPos,
                (int) newStdPos,
                0,
                0,
                oldAcc * 100,
                newAcc * 100,
                oldRankedScore,
                newRankedScore,
                oldPP,
                newPP,
                NewScore?.Id ?? 0,
                AchievementProcessor.ProcessAchievements(_factory, scores.ScoreOwner, scores, bm, cg.GetSets()[0], oldStd, newStd)
            );
            
            pr.LeaderboardStd = newStd;
            await _ev.RunEvent(EventType.BanchoUserStatsRequest, new BanchoUserStatsRequestArgs
            {
                userIds = new List<int>{scores.ScoreOwner.Id},
                pr      = pr
            });

            return Ok($"beatmapId:{bm.BeatmapID}|beatmapSetId:{bm.ParentSetID}|beatmapPlaycount:0|beatmapPasscount:0|approvedDate:\n\n" + 
                      bmChart.ToOsuString() + "\n" + overallChart.ToOsuString());
        }
        
        #endregion

        #region GET /web/osu-getreplay.php

        [HttpGet("osu-getreplay.php")]
        public IActionResult GetReplay(
            [FromQuery(Name = "c")] int replayId,
            [FromQuery(Name = "m")] PlayMode mode,
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
            )
        {
            Users user = Users.GetUser(_factory, Users.GetUserId(_factory, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            Scores s = Scores.GetScore(_factory, replayId);
            if (s == null)
                return NotFound();

            return File(System.IO.File.OpenRead("data/replays/" + s.ReplayMd5), "binary/octet-stream", s.ReplayMd5);
        }

        #endregion

        #region GET /web/osu-search.php

        [HttpGet("osu-search.php")]
        public IActionResult GetSearchDIRECT(
            [FromQuery(Name = "m")] int playMode,
            [FromQuery(Name = "r")] int rankedStatus,
            [FromQuery(Name = "p")] int page,
            [FromQuery(Name = "q")] string query,
            
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            Users user = Users.GetUser(_factory, Users.GetUserId(_factory, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            string cache_hash = Hex.ToHex(Crypto.GetMd5($"m{playMode}r{rankedStatus}p{page}q{query}"));

            string cachedData = _cache.GetCachedString($"sora:DirectSearches:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Cheesegull cg = new Cheesegull(_config);
            try
            {
                cg.Search(query, rankedStatus, playMode, page);
            }
            catch
            {
                // Ignored
            }

            Response.ContentType = "text/plain";

            _cache.CacheString($"sora:DirectSearches:{cache_hash}", cachedData = cg.ToDirect(), 600);
            
            return Ok(cachedData);
        }

        #endregion

        #region GET /web/osu-search-set.php
        [HttpGet("osu-search-set.php")]
        public IActionResult GetDirectNP(
            [FromQuery(Name = "s")] int setId,
            [FromQuery(Name = "b")] int beatmapId,

            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            Users user = Users.GetUser(_factory, userName);
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            string cache_hash = Hex.ToHex(Crypto.GetMd5($"s{setId}|b{beatmapId}"));

            string cachedData = _cache.GetCachedString($"sora:DirectNP:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Cheesegull cg = new Cheesegull(_config);
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

            _cache.CacheString($"sora:DirectNP:{cache_hash}", cachedData = cg.ToNP(), 600);

            return Ok(cachedData);
        }
        #endregion
        
        #region GET /web/check-updates.php

        [HttpGet("check-updates.php")]
        public IActionResult CheckUpdates(
            [FromQuery] string action,
            [FromQuery(Name = "stream")] string qstream,
            [FromQuery]ulong time)
        {
            string answer;
            if ((answer = _cache.GetCachedString("sora:updater:" + action + qstream)) != null)
                return Ok(answer);
            
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create($"http://osu.ppy.sh/web/check-updates.php?action={action}&stream={qstream}&time={time}");
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using Stream          stream   = response.GetResponseStream();
            using StreamReader    reader   = new StreamReader(stream ?? throw new Exception("Request Failed!"));
            
            string result = reader.ReadToEnd();
            _cache.CacheString("sora:updater:" + action + qstream, result, 36000);
            return Ok(result);
        }
        #endregion
        
        #region POST /web/osu-screenshot.php

        [HttpPost("osu-screenshot.php")]
        public IActionResult UploadScreenshot()
        {
            if (!Directory.Exists("data/screenshots"))
                Directory.CreateDirectory("data/screenshots");
            
            IFormFile screenshot = Request.Form.Files.GetFile("ss");
            string    Randi      = Crypto.RandomString(16);
            using(Stream stream = screenshot.OpenReadStream())
            using (FileStream fs = System.IO.File.OpenWrite($"data/screenshots/{Randi}"))
            {
                Image.FromStream(stream)
                      .Save(fs, ImageFormat.Jpeg);
            }

            return Ok($"http://{_config.Server.Hostname}/ss/{Randi}");
        }
        
        #endregion
    }
}