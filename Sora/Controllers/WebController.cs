using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sora.Allocation;
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
        private readonly Cache _cache;
        private readonly Config _config;
        private readonly EventManager _ev;
        private readonly SoraDbContextFactory _factory;
        private readonly PresenceService _ps;
        private readonly Bot.Sora _sora;

        public WebController(SoraDbContextFactory factory,
            EventManager ev,
            Cache cache,
            Config config,
            Bot.Sora sora,
            PresenceService ps)
        {
            _factory = factory;
            _ev = ev;
            _cache = cache;
            _config = config;
            _sora = sora;
            _ps = ps;
        }

        #region GET /web/

        [HttpGet]
        public IActionResult Index() => Ok("ERR: you sneaky little mouse :3");

        #endregion

        #region GET /web/osu-osz2-getscores.php

        [HttpGet("osu-osz2-getscores.php")]
        public IActionResult GetScoreResult(
            [FromQuery(Name = "v")] ScoreboardType type,
            [FromQuery(Name = "c")] string fileMD5,
            [FromQuery(Name = "f")] string f,
            [FromQuery(Name = "m")] PlayMode playMode,
            [FromQuery(Name = "i")] int i,
            [FromQuery(Name = "mods")] Mod mods,
            [FromQuery(Name = "us")] string us,
            [FromQuery(Name = "ha")] string pa)
        {
            var user = Users.GetUser(_factory, Users.GetUserId(_factory, us));
            if (user?.IsPassword(pa) != true)
                return Ok("error: pass");

            var cache_hash =
                Hex.ToHex(
                    Crypto.GetMd5(
                        $"{fileMD5}{playMode}{mods}{type}{user.Id}{user.Username}"
                    )
                );

            var cachedData = _cache.Get<string>($"sora:Scoreboards:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            var sboard = new Scoreboard(
                _factory, _config,
                fileMD5, user, playMode,
                (mods & Mod.Relax) != 0,
                type == ScoreboardType.Friends,
                type == ScoreboardType.Country,
                type == ScoreboardType.Mods,
                mods
            );

            _cache.Set($"sora:Scoreboards:{cache_hash}", cachedData = sboard.ToOsuString(), TimeSpan.FromSeconds(30));

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

            var (b, scores) = ScoreSubmissionParser.ParseScore(_factory, score, iv, osuver);

            if (scores.UserId == -1)
                return Ok("error: pass");

            if (scores.ScoreOwner == null)
                scores.ScoreOwner = Users.GetUser(_factory, scores.UserId);

            if (scores.ScoreOwner == null)
                return Ok("error: pass");

            if (!scores.ScoreOwner.IsPassword(pass))
                return Ok("error: pass");

            var isRelaxing = (scores.Mods & Mod.Relax) != 0;
            var pr = _ps.GetPresence(scores.ScoreOwner.Id);

            if (!b || !RankedMods.IsRanked(scores.Mods))
            {
                if (isRelaxing)
                {
                    var rx = LeaderboardRx.GetLeaderboard(_factory.Get(), scores.ScoreOwner);
                    
                    rx.IncreasePlaycount(scores.PlayMode);
                    rx.IncreaseScore((ulong) scores.TotalScore, false, scores.PlayMode);
                    
                    rx.SaveChanges(_factory);
                }
                else
                {
                    var std = LeaderboardStd.GetLeaderboard(_factory.Get(), scores.ScoreOwner);
                    
                    std.IncreasePlaycount(scores.PlayMode);
                    std.IncreaseScore((ulong) scores.TotalScore, false, scores.PlayMode);
                    
                    std.SaveChanges(_factory);
                }

                await _ev.RunEvent(
                    EventType.BanchoUserStatsRequest,
                    new BanchoUserStatsRequestArgs {userIds = new List<int> {scores.ScoreOwner.Id}, pr = pr}
                );

                return Ok("Thanks for your hard work!");
            }

            var ReplayFile = Request.Form.Files.GetFile("score");

            if (!Directory.Exists("data/replays"))
                Directory.CreateDirectory("data/replays");

            await using (var m = new MemoryStream())
            {
                ReplayFile.CopyTo(m);
                m.Position = 0;
                scores.ReplayMd5 = Hex.ToHex(Crypto.GetMd5(m)) ?? string.Empty;
                if (!string.IsNullOrEmpty(scores.ReplayMd5))
                    await using (var replayFile = System.IO.File.Create($"data/replays/{scores.ReplayMd5}"))
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
            scores.ComputePerformancePoints();
            if (isRelaxing)
                scores.Mods |= Mod.Relax;

            var oldScore = Scores.GetScores(
                _factory,
                scores.FileMd5,
                scores.ScoreOwner,
                scores.PlayMode,
                isRelaxing,
                false,
                false,
                false,
                scores.Mods,
                true
            ).FirstOrDefault();

            var oldStd = LeaderboardStd.GetLeaderboard(_factory.Get(), scores.ScoreOwner);
            var oldStdPos = oldStd.GetPosition(_factory.Get(), scores.PlayMode);
            
            var oldAcc = oldStd.GetAccuracy(_factory.Get(), scores.PlayMode);
            double newAcc;

            if (oldScore != null && oldScore.TotalScore <= scores.TotalScore)
            {
                using var db = _factory.GetForWrite();
                db.Context.Scores.Remove(oldScore);
                System.IO.File.Delete($"data/replays/{oldScore.ReplayMd5}");

                Scores.InsertScore(_factory, scores);
            }
            else if (oldScore == null)
            {
                Scores.InsertScore(_factory, scores);
            }
            else
            {
                System.IO.File.Delete($"data/replays/{scores.ReplayMd5}");
            }

            if (isRelaxing)
            {
                var rx = LeaderboardRx.GetLeaderboard(_factory.Get(), scores.ScoreOwner);
                
                rx.IncreasePlaycount(scores.PlayMode);
                rx.IncreaseScore((ulong) scores.TotalScore, true, scores.PlayMode);
                rx.IncreaseScore((ulong) scores.TotalScore, false, scores.PlayMode);

                rx.UpdatePP(_factory.Get(), scores.PlayMode);

                rx.SaveChanges(_factory);
                
                pr["LB_RX"] = rx;
                
                await _ev.RunEvent(
                    EventType.BanchoUserStatsRequest,
                    new BanchoUserStatsRequestArgs {userIds = new List<int> {scores.ScoreOwner.Id}, pr = pr}
                );
            }
            else
            {
                var std = LeaderboardStd.GetLeaderboard(_factory.Get(), scores.ScoreOwner);
                
                std.IncreasePlaycount(scores.PlayMode);
                std.IncreaseScore((ulong) scores.TotalScore, true, scores.PlayMode);
                std.IncreaseScore((ulong) scores.TotalScore, false, scores.PlayMode);

                std.UpdatePP(_factory.Get(), scores.PlayMode);

                std.SaveChanges(_factory);
            }

            var newStd = LeaderboardStd.GetLeaderboard(_factory.Get(), scores.ScoreOwner);
            var newStdPos = newStd.GetPosition(_factory.Get(), scores.PlayMode);
            newAcc = newStd.GetAccuracy(_factory.Get(), scores.PlayMode);

            var NewScore = Scores.GetScores(
                _factory,
                scores.FileMd5,
                scores.ScoreOwner,
                scores.PlayMode,
                isRelaxing,
                false,
                false,
                false,
                scores.Mods,
                true
            ).FirstOrDefault();

            var cg = new Cheesegull(_config);
            cg.SetBM(scores.FileMd5);

            var sets = cg.GetSets();
            var bm = sets?[0].ChildrenBeatmaps.First(x => x.FileMD5 == scores.FileMd5) ?? new CheesegullBeatmap();

            ulong oldRankedScore;
            ulong newRankedScore;

            double oldPP;
            double newPP;

            switch (scores.PlayMode)
            {
                case PlayMode.Osu:
                    oldRankedScore = oldStd.RankedScoreOsu;
                    newRankedScore = newStd.RankedScoreOsu;

                    oldPP = oldStd.PerformancePointsOsu;
                    newPP = newStd.PerformancePointsOsu;
                    break;
                case PlayMode.Taiko:
                    oldRankedScore = oldStd.RankedScoreTaiko;
                    newRankedScore = newStd.RankedScoreTaiko;

                    oldPP = oldStd.PerformancePointsTaiko;
                    newPP = newStd.PerformancePointsTaiko;
                    break;
                case PlayMode.Ctb:
                    oldRankedScore = oldStd.RankedScoreCtb;
                    newRankedScore = newStd.RankedScoreCtb;

                    oldPP = oldStd.PerformancePointsCtb;
                    newPP = newStd.PerformancePointsCtb;
                    break;
                case PlayMode.Mania:
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
                    $"[http://{_config.Server.ScreenshotHostname}/{scores.ScoreOwner.Id} {scores.ScoreOwner.Username}] " +
                    $"has reached #1 on [https://osu.ppy.sh/b/{bm.BeatmapID} {sets?[0].Title} [{bm.DiffName}]] " +
                    $"using {ModUtil.ToString(NewScore.Mods)} " +
                    $"Good job! +{NewScore.PerformancePoints:F}PP",
                    "#announce",
                    false
                );

            Logger.Info(
                $"{L_COL.RED}{scores.ScoreOwner.Username}",
                $"{L_COL.PURPLE}( {scores.ScoreOwner.Id} ){L_COL.WHITE}",
                $"has just submitted a Score! he earned {L_COL.BLUE}{NewScore?.PerformancePoints:F}PP",
                $"{L_COL.WHITE}with an Accuracy of {L_COL.RED}{NewScore?.Accuracy * 100:F}",
                $"{L_COL.WHITE}on {L_COL.YELLOW}{sets?[0].Title} [{bm.DiffName}]",
                $"{L_COL.WHITE}using {L_COL.BLUE}{ModUtil.ToString(NewScore?.Mods ?? Mod.None)}"
            );

            var bmChart = new Chart(
                "beatmap",
                "Beatmap Ranking",
                $"https://osu.ppy.sh/b/{bm.BeatmapID}",
                oldScore?.Position ?? 0,
                NewScore?.Position ?? 0,
                oldScore?.MaxCombo ?? 0,
                NewScore?.MaxCombo ?? 0,
                oldScore?.Accuracy * 100 ?? 0,
                NewScore?.Accuracy * 100 ?? 0,
                (ulong) (oldScore?.TotalScore ?? 0),
                (ulong) (NewScore?.TotalScore ?? 0),
                oldScore?.PerformancePoints ?? 0,
                NewScore?.PerformancePoints ?? 0,
                NewScore?.Id ?? 0
            );

            cg.SetBMSet(bm.ParentSetID);

            var overallChart = new Chart(
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
                AchievementProcessor.ProcessAchievements(
                    _factory, scores.ScoreOwner, scores, bm, cg.GetSets()[0], oldStd, newStd
                )
            );

            pr["LB_STD"] = newStd;
            
            await _ev.RunEvent(
                EventType.BanchoUserStatsRequest,
                new BanchoUserStatsRequestArgs {userIds = new List<int> {scores.ScoreOwner.Id}, pr = pr}
            );

            return Ok(
                $"beatmapId:{bm.BeatmapID}|beatmapSetId:{bm.ParentSetID}|beatmapPlaycount:0|beatmapPasscount:0|approvedDate:\n\n" +
                bmChart.ToOsuString() + "\n" + overallChart.ToOsuString()
            );
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
            var user = Users.GetUser(_factory, Users.GetUserId(_factory, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            var s = Scores.GetScore(_factory, replayId);
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
            var user = Users.GetUser(_factory, Users.GetUserId(_factory, userName));
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            var cache_hash = Hex.ToHex(Crypto.GetMd5($"m{playMode}r{rankedStatus}p{page}q{query}"));

            var cachedData = _cache.Get<string>($"sora:DirectSearches:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            var cg = new Cheesegull(_config);
            try
            {
                cg.Search(query, rankedStatus, playMode, page);
            } catch
            {
                // Ignored
            }

            Response.ContentType = "text/plain";

            _cache.Set($"sora:DirectSearches:{cache_hash}", cachedData = cg.ToDirect(), TimeSpan.FromMinutes(10));

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
            var user = Users.GetUser(_factory, userName);
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            var cache_hash = Hex.ToHex(Crypto.GetMd5($"s{setId}|b{beatmapId}"));

            var cachedData = _cache.Get<string>($"sora:DirectNP:{cache_hash}");

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            var cg = new Cheesegull(_config);
            try
            {
                if (setId != 0)
                    cg.SetBMSet(setId);
                else
                    cg.SetBM(beatmapId);
            } catch
            {
                // ignored
            }

            Response.ContentType = "text/plain";

            _cache.Set($"sora:DirectNP:{cache_hash}", cachedData = cg.ToNP(), TimeSpan.FromMinutes(10));

            return Ok(cachedData);
        }

        #endregion

        #region GET /web/check-updates.php

        [HttpGet("check-updates.php")]
        public IActionResult CheckUpdates(
            [FromQuery] string action,
            [FromQuery(Name = "stream")] string qstream,
            [FromQuery] ulong time)
        {
            string answer;
            if ((answer = _cache.Get<string>("sora:updater:" + action + qstream)) != null)
                return Ok(answer);

            var request = (HttpWebRequest) WebRequest.Create(
                $"http://osu.ppy.sh/web/check-updates.php?action={action}&stream={qstream}&time={time}"
            );
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var response = (HttpWebResponse) request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream ?? throw new Exception("Request Failed!"));

            var result = reader.ReadToEnd();
            _cache.Set("sora:updater:" + action + qstream, result, TimeSpan.FromDays(1));
            return Ok(result);
        }

        #endregion

        #region POST /web/osu-screenshot.php

        [HttpPost("osu-screenshot.php")]
        public IActionResult UploadScreenshot()
        {
            if (!Directory.Exists("data/screenshots"))
                Directory.CreateDirectory("data/screenshots");

            var screenshot = Request.Form.Files.GetFile("ss");
            var Randi = Crypto.RandomString(16);
            using (var stream = screenshot.OpenReadStream())
            using (var fs = System.IO.File.OpenWrite($"data/screenshots/{Randi}"))
            {
                Image.FromStream(stream)
                     .Save(fs, ImageFormat.Jpeg);
            }

            return Ok($"http://{_config.Server.ScreenshotHostname}/ss/{Randi}");
        }

        #endregion
    }
}
