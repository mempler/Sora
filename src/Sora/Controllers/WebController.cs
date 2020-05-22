using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using osu.Game.Overlays.KeyBinding;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Allocation;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Objects.Scores;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/web/")]
    public class WebController : Controller
    {
        private readonly Cache _cache;
        private readonly Config _config;
        private readonly Bot.Sora _sora;
        private readonly SoraDbContext _ctx;
        private readonly EventManager _ev;
        private readonly PresenceService _ps;
        private readonly Pisstaube _pisstaube;

        public WebController(
            SoraDbContext ctx,
            EventManager ev,
            PresenceService ps,
            Cache cache,
            Config config,
            Bot.Sora sora,
            Pisstaube pisstaube)
        {
            _ctx = ctx;
            _ev = ev;
            _ps = ps;
            _cache = cache;
            _config = config;
            _sora = sora;
            _pisstaube = pisstaube;
        }

        #region GET /web/

        [HttpGet]
        public IActionResult Index() => Ok("ERR: you sneaky little mouse :3");

        #endregion

        #region GET /web/osu-search.php

        [HttpGet("osu-search.php")]
        public async Task<IActionResult> GetSearchDirect(
            [FromQuery(Name = "m")] int playMode,
            [FromQuery(Name = "r")] int rankedStatus,
            [FromQuery(Name = "p")] int page,
            [FromQuery(Name = "q")] string query,
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            var user = await DbUser.GetDbUser(_ctx, userName);
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            var cacheHash = Hex.ToHex(Crypto.GetMd5($"m{playMode}r{rankedStatus}p{page}q{query}"));
            
            if (_cache.TryGet($"sora:DirectSearches:{cacheHash}", out string cachedData))
                return Ok(cachedData);

            Response.ContentType = "text/plain";

            var searchResult = await _pisstaube.SearchAsync(query, rankedStatus, playMode, page);

            _cache.Set($"sora:DirectSearches:{cacheHash}", cachedData = searchResult.ToDirect(),
                TimeSpan.FromMinutes(10));

            return Ok(cachedData);
        }

        #endregion

        #region GET /web/osu-search-set.php

        [HttpGet("osu-search-set.php")]
        public async Task<IActionResult> GetDirectNp(
            [FromQuery(Name = "s")] int setId,
            [FromQuery(Name = "b")] int beatmapId,
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            var user = await DbUser.GetDbUser(_ctx, userName);
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            var cacheHash = Hex.ToHex(Crypto.GetMd5($"s{setId}|b{beatmapId}"));
            
            if (_cache.TryGet($"sora:DirectNP:{cacheHash}", out string cachedData))
                return Ok(cachedData);

            if (!string.IsNullOrEmpty(cachedData))
                return Ok(cachedData);

            Response.ContentType = "text/plain";
            
            if (setId != 0)
            {
                var set = await _pisstaube.FetchBeatmapSetAsync(setId);
                _cache.Set($"sora:DirectNP:{cacheHash}", cachedData = BeatmapSet.ToNP(set),
                    TimeSpan.FromMinutes(10));
            }
            else
            {
                var bm = await _pisstaube.FetchBeatmapAsync(beatmapId);
                var set = await _pisstaube.FetchBeatmapSetAsync(bm.ParentSetID);
                _cache.Set($"sora:DirectNP:{cacheHash}", cachedData = BeatmapSet.ToNP(set),
                    TimeSpan.FromMinutes(10));
            }

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
            if (_cache.TryGet("sora:updater:" + action + qstream, out string answer))
                return Ok(answer);

            var request = (HttpWebRequest) WebRequest.Create(
                $"https://1.1.1.1/web/check-updates.php?action={action}&stream={qstream}&time={time}"
            );
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Host = "osu.ppy.sh";
            request.UserAgent = "osu!";

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
            var randi = Crypto.RandomString(16);
            using (var stream = screenshot.OpenReadStream())
            {
                using var fs = System.IO.File.OpenWrite($"data/screenshots/{randi}");
                Image.FromStream(stream)
                     .Save(fs, ImageFormat.Jpeg);
            }

            return Ok($"http://{_config.Server.ScreenShotHostname}/ss/{randi}");
        }

        #endregion

        #region GET /web/osu-getreplay.php

        [HttpGet("osu-getreplay.php")]
        public async Task<IActionResult> GetReplay(
            [FromQuery(Name = "c")] int replayId,
            [FromQuery(Name = "m")] PlayMode mode,
            [FromQuery(Name = "u")] string userName,
            [FromQuery(Name = "h")] string pass
        )
        {
            var user = await DbUser.GetDbUser(_ctx, userName);
            if (user == null)
                return Ok("err: pass");

            if (!user.IsPassword(pass))
                return Ok("err: pass");

            var s = await DbScore.GetScore(_ctx, replayId);
            if (s == null)
                return NotFound();

            return File(System.IO.File.OpenRead("data/replays/" + s.ReplayMd5), "binary/octet-stream", s.ReplayMd5);
        }

        #endregion
        
        #region POST /web/osu-submit-modular-selector.php

        [HttpPost("osu-submit-modular-selector.php")]
        public async Task<IActionResult> PostSubmitModular()
        {
            if (!Directory.Exists("data/replays"))
                Directory.CreateDirectory("data/replays");
            
            string encScore = Request.Form["score"];
            string iv = Request.Form["iv"];
            string osuver = Request.Form["osuver"];
            string passwd = Request.Form["pass"];

            var (pass, score) = ScoreSubmissionParser.ParseScore(encScore, iv, osuver);
            var dbUser = await DbUser.GetDbUser(_ctx, score.UserName);

            if (dbUser == null)
                return Ok("error: pass");
            
            if (!dbUser.IsPassword(passwd))
                return Ok("error: pass");

            if (!_ps.TryGet(dbUser.Id, out var pr))
                return Ok("error: pass"); // User not logged in in Bancho!

            if (!pass || !RankedMods.IsRanked(score.Mods))
            {
                var lb = await DbLeaderboard.GetLeaderboardAsync(_ctx, dbUser);

                lb.IncreasePlaycount(score.PlayMode);
                lb.IncreaseScore((ulong) score.TotalScore, false, score.PlayMode);

                await lb.SaveChanges(_ctx);
                
                // Send to other People
                await _ev.RunEvent(
                    EventType.BanchoUserStatsRequest,
                    new BanchoUserStatsRequestArgs {UserIds = new List<int> {score.Id}, Pr = pr}
                );

                // Send to self
                await _ev.RunEvent(
                    EventType.BanchoSendUserStatus,
                    new BanchoSendUserStatusArgs {Status = pr.Status, Pr = pr}
                );

                return Ok("Thanks for your hard work! onii-chyan~"); // even though, we're Sora, we can still be cute!
            }

            var replayFileData = Request.Form.Files.GetFile("score");

            var dbScore = new DbScore
            {
                Accuracy = score.ComputeAccuracy(),
                Count100 = score.Count100,
                Count50 = score.Count50,
                Count300 = score.Count300,
                Date = score.Date,
                Mods = score.Mods,
                CountGeki = score.CountGeki,
                CountKatu = score.CountKatu,
                CountMiss = score.CountMiss,
                FileMd5 = score.FileMd5,
                MaxCombo = score.MaxCombo,
                PlayMode = score.PlayMode,
                ScoreOwner = dbUser,
                TotalScore = score.TotalScore,
                UserId = dbUser.Id
            };

            await _pisstaube.DownloadBeatmapAsync(dbScore.FileMd5);

            await using (var m = new MemoryStream())
            {
                replayFileData.CopyTo(m);
                m.Position = 0;
                dbScore.ReplayMd5 = Hex.ToHex(Crypto.GetMd5(m)) ?? string.Empty;
                if (!string.IsNullOrEmpty(dbScore.ReplayMd5))
                {
                    await using var replayFile = System.IO.File.Create($"data/replays/{dbScore.ReplayMd5}");
                    m.Position = 0;
                    m.WriteTo(replayFile);
                    m.Close();
                    replayFile.Close();
                }
            }

            dbScore.PerformancePoints = dbScore.ComputePerformancePoints();

            var oldScore = await DbScore.GetLatestScore(_ctx, dbScore);

            var oldLb = await DbLeaderboard.GetLeaderboardAsync(_ctx, dbScore.ScoreOwner);
            var oldStdPos = oldLb.GetPosition(_ctx, dbScore.PlayMode);
            
            var oldAcc = oldLb.GetAccuracy(_ctx, dbScore.PlayMode);
            double newAcc;

            if (oldScore != null && oldScore.TotalScore <= dbScore.TotalScore)
            {
                _ctx.Remove(oldScore);
                System.IO.File.Delete($"data/replays/{oldScore.ReplayMd5}");

                await DbScore.InsertScore(_ctx, dbScore);
            }
            else if (oldScore == null)
            {
                await DbScore.InsertScore(_ctx, dbScore);
            }
            else
            {
                System.IO.File.Delete($"data/replays/{oldScore.ReplayMd5}");
            }

            var newlb = await DbLeaderboard.GetLeaderboardAsync(_ctx, dbScore.ScoreOwner);

            newlb.IncreasePlaycount(dbScore.PlayMode);
            newlb.IncreaseScore((ulong) dbScore.TotalScore, true, dbScore.PlayMode);
            newlb.IncreaseScore((ulong) dbScore.TotalScore, false, dbScore.PlayMode);

            newlb.UpdatePp(_ctx, dbScore.PlayMode);

            await newlb.SaveChanges(_ctx);

            var newStdPos = newlb.GetPosition(_ctx, dbScore.PlayMode);
            newAcc = newlb.GetAccuracy(_ctx, dbScore.PlayMode);

            var newScore = await DbScore.GetLatestScore(_ctx, dbScore);

            var set = await _pisstaube.FetchBeatmapSetAsync(dbScore.FileMd5);

            var bm = set?.ChildrenBeatmaps.First(x => x.FileMD5 == dbScore.FileMd5) ?? new Beatmap();

            ulong oldRankedScore;
            ulong newRankedScore;

            double oldPp;
            double newPp;

            switch (dbScore.PlayMode)
            {
                case PlayMode.Osu:
                    oldRankedScore = oldLb.RankedScoreOsu;
                    newRankedScore = newlb.RankedScoreOsu;

                    oldPp = oldLb.PerformancePointsOsu;
                    newPp = newlb.PerformancePointsOsu;
                    break;
                case PlayMode.Taiko:
                    oldRankedScore = oldLb.RankedScoreTaiko;
                    newRankedScore = newlb.RankedScoreTaiko;

                    oldPp = oldLb.PerformancePointsTaiko;
                    newPp = newlb.PerformancePointsTaiko;
                    break;
                case PlayMode.Ctb:
                    oldRankedScore = oldLb.RankedScoreCtb;
                    newRankedScore = newlb.RankedScoreCtb;

                    oldPp = oldLb.PerformancePointsCtb;
                    newPp = newlb.PerformancePointsCtb;
                    break;
                case PlayMode.Mania:
                    oldRankedScore = oldLb.RankedScoreMania;
                    newRankedScore = newlb.RankedScoreMania;

                    oldPp = oldLb.PerformancePointsMania;
                    newPp = newlb.PerformancePointsMania;
                    break;
                default:
                    return Ok("");
            }

            var newScorePosition = newScore != null ? await newScore.Position(_ctx) : 0;
            var oldScorePosition = oldScore != null ? await oldScore.Position(_ctx) : 0;
            
            if (newScorePosition == 1)
                _sora.SendMessage(
                    $"[http://{_config.Server.ScreenShotHostname}/{dbScore.ScoreOwner.Id} {dbScore.ScoreOwner.UserName}] " +
                    $"has reached #1 on [https://osu.ppy.sh/b/{bm.BeatmapID} {set?.Title} [{bm.DiffName}]] " +
                    $"using {ModUtil.ToString(newScore.Mods)} " +
                    $"Good job! +{newScore.PerformancePoints:F}PP",
                    "#announce",
                    false
                );

            Logger.Info(
                $"{LCol.RED}{dbScore.ScoreOwner.UserName}",
                $"{LCol.PURPLE}( {dbScore.ScoreOwner.Id} ){LCol.WHITE}",
                $"has just submitted a Score! he earned {LCol.BLUE}{newScore?.PerformancePoints:F}PP",
                $"{LCol.WHITE}with an Accuracy of {LCol.RED}{newScore?.Accuracy * 100:F}",
                $"{LCol.WHITE}on {LCol.YELLOW}{set?.Title} [{bm.DiffName}]",
                $"{LCol.WHITE}using {LCol.BLUE}{ModUtil.ToString(newScore?.Mods ?? Mod.None)}"
            );

            var bmChart = new Chart(
                "beatmap",
                "Beatmap Ranking",
                $"https://osu.ppy.sh/b/{bm.BeatmapID}",
                oldScorePosition,
                newScorePosition,
                oldScore?.MaxCombo ?? 0,
                newScore?.MaxCombo ?? 0,
                oldScore?.Accuracy * 100 ?? 0,
                newScore?.Accuracy * 100 ?? 0,
                (ulong) (oldScore?.TotalScore ?? 0),
                (ulong) (newScore?.TotalScore ?? 0),
                oldScore?.PerformancePoints ?? 0,
                newScore?.PerformancePoints ?? 0,
                newScore?.Id ?? 0
            );

            var overallChart = new Chart(
                "overall",
                "Global Ranking",
                $"https://osu.ppy.sh/u/{dbUser.Id}",
                (int) oldStdPos,
                (int) newStdPos,
                0,
                0,
                oldAcc * 100,
                newAcc * 100,
                oldRankedScore,
                newRankedScore,
                oldPp,
                newPp,
                newScore?.Id ?? 0,
                AchievementProcessor.ProcessAchievements(
                    _ctx, dbScore.ScoreOwner, score, bm, set, oldLb, newlb
                )
            );

            pr["LB"] = newlb;
            pr.Stats.Accuracy = (float) newlb.GetAccuracy(_ctx, score.PlayMode);
            pr.Stats.Position = newlb.GetPosition(_ctx, score.PlayMode);
            switch (score.PlayMode)
            {
                case PlayMode.Osu:
                    pr.Stats.PerformancePoints = (ushort) newlb.PerformancePointsOsu;
                    pr.Stats.TotalScore = (ushort) newlb.TotalScoreOsu;
                    pr.Stats.RankedScore = (ushort) newlb.RankedScoreOsu;
                    pr.Stats.PlayCount = (ushort) newlb.PlayCountOsu;
                    break;
                case PlayMode.Taiko:
                    pr.Stats.PerformancePoints = (ushort) newlb.PerformancePointsTaiko;
                    pr.Stats.TotalScore = (ushort) newlb.TotalScoreTaiko;
                    pr.Stats.RankedScore = (ushort) newlb.RankedScoreTaiko;
                    pr.Stats.PlayCount = (ushort) newlb.PlayCountTaiko;
                    break;
                case PlayMode.Ctb:
                    pr.Stats.PerformancePoints = (ushort) newlb.PerformancePointsCtb;
                    pr.Stats.TotalScore = (ushort) newlb.TotalScoreCtb;
                    pr.Stats.RankedScore = (ushort) newlb.RankedScoreCtb;
                    pr.Stats.PlayCount = (ushort) newlb.PlayCountCtb;
                    break;
                case PlayMode.Mania:
                    pr.Stats.PerformancePoints = (ushort) newlb.PerformancePointsMania;
                    pr.Stats.TotalScore = (ushort) newlb.TotalScoreMania;
                    pr.Stats.RankedScore = (ushort) newlb.RankedScoreMania;
                    pr.Stats.PlayCount = (ushort) newlb.PlayCountMania;
                    break;
            }
            
            // Send to other People
            await _ev.RunEvent(
                EventType.BanchoUserStatsRequest,
                new BanchoUserStatsRequestArgs {UserIds = new List<int> {score.Id}, Pr = pr}
            );

            // Send to self
            await _ev.RunEvent(
                EventType.BanchoSendUserStatus,
                new BanchoSendUserStatusArgs {Status = pr.Status, Pr = pr}
            );

            return Ok(
                $"beatmapId:{bm.BeatmapID}|beatmapSetId:{bm.ParentSetID}|beatmapPlaycount:0|beatmapPasscount:0|approvedDate:\n\n" +
                bmChart.ToOsuString() + "\n" + overallChart.ToOsuString()
            );
        }

        #endregion

        #region GET /web/osu-osz2-getscores.php

        [HttpGet("osu-osz2-getscores.php")]
        public async Task<IActionResult> GetScoreResult(
            [FromQuery(Name = "v")] ScoreboardType type,
            [FromQuery(Name = "c")] string fileMd5,
            [FromQuery(Name = "f")] string f,
            [FromQuery(Name = "m")] PlayMode playMode,
            [FromQuery(Name = "i")] int i,
            [FromQuery(Name = "mods")] Mod mods,
            [FromQuery(Name = "us")] string us,
            [FromQuery(Name = "ha")] string pa,
            [FromServices] IServiceProvider serviceProvider)
        {
            try
            {
                var dbUser = await DbUser.GetDbUser(_ctx, us);
                var user = dbUser?.ToUser();
                if (dbUser?.IsPassword(pa) != true)
                    return Ok("error: pass");

                var cacheHash =
                    Hex.ToHex(
                        Crypto.GetMd5(
                            $"{fileMd5}{playMode}{mods}{type}{user.Id}{user.UserName}"
                        )
                    );

                if (_cache.TryGet($"sora:Scoreboards:{cacheHash}", out string cachedData))
                    return Ok(cachedData);

                var scores = await DbScore.GetScores(_ctx, fileMd5, dbUser, playMode,
                                                     type == ScoreboardType.Friends,
                                                     type == ScoreboardType.Country,
                                                     type == ScoreboardType.Mods,
                                                     mods);

                var beatmap = DbBeatmap.GetBeatmap(_ctx, fileMd5);
                BeatmapSet apiSet;
                if (beatmap == null)
                {
                    apiSet = await _pisstaube.FetchBeatmapSetAsync(fileMd5);
                    
                    if (apiSet == null)
                        goto JustContinue;

                    var beatmaps = DbBeatmap.FromBeatmapSet(apiSet).ToList();
                    var beatmapChecksums = beatmaps.Select(s => s.FileMd5);
                    var dbBeatmaps =
                        _ctx.Beatmaps.Where(rset => beatmapChecksums.Any(lFileMd5 => rset.FileMd5 == lFileMd5))
                            .ToList();
                    
                    var concurrentLock = new object();
                    var pool = serviceProvider.GetRequiredService<DbContextPool<SoraDbContext>>();
                    Task.WaitAll(beatmaps.Select(rawBeatmap => Task.Run(async () =>
                    {
                        var context = pool.Rent();
    
                        var dbBeatmap = dbBeatmaps.FirstOrDefault(s => s.FileMd5 == rawBeatmap.FileMd5);

                        if (dbBeatmap != null && (dbBeatmap.Flags & DbBeatmapFlags.RankedFreeze) != 0)
                        {
                            rawBeatmap.RankedStatus = dbBeatmap.RankedStatus;
                            rawBeatmap.Flags        = dbBeatmap.Flags;
                        }

                        context.Beatmaps.AddOrUpdate(rawBeatmap);
                        await context.SaveChangesAsync();

                        pool.Return(context);
                    })).ToArray());
                    
                    beatmap = beatmaps.FirstOrDefault(s => s.FileMd5 == fileMd5);
                }

                JustContinue:
                var ownScore = await DbScore.GetLatestScore(_ctx, new DbScore
                {
                    FileMd5 = fileMd5,
                    UserId = user.Id,
                    PlayMode = playMode,
                    TotalScore = 0
                });

                var sScores = scores.Select(s => new Score
                {
                    Count100 = s.Count100,
                    Count50 = s.Count50,
                    Count300 = s.Count300,
                    Date = s.Date,
                    Mods = s.Mods,
                    CountGeki = s.CountGeki,
                    CountKatu = s.CountKatu,
                    CountMiss = s.CountMiss,
                    FileMd5 = s.FileMd5,
                    MaxCombo = s.MaxCombo,
                    PlayMode = s.PlayMode,
                    TotalScore = s.TotalScore,
                    UserId = s.UserId,
                    UserName = s.ScoreOwner.UserName,
                }).ToList();

                // Fetch the correct position for sScore
                for (var j = 0; j < scores.Count; j++)
                {
                    sScores[j].Position = await scores[j].Position(_ctx);
                }

                Score ownsScore = null;
                if (ownScore != null)
                    ownsScore = new Score
                    {
                        Count100 = ownScore.Count100,
                        Count50 = ownScore.Count50,
                        Count300 = ownScore.Count300,
                        Date = ownScore.Date,
                        Mods = ownScore.Mods,
                        CountGeki = ownScore.CountGeki,
                        CountKatu = ownScore.CountKatu,
                        CountMiss = ownScore.CountMiss,
                        FileMd5 = ownScore.FileMd5,
                        MaxCombo = ownScore.MaxCombo,
                        PlayMode = ownScore.PlayMode,
                        TotalScore = ownScore.TotalScore,
                        UserId = dbUser.Id,
                        UserName = dbUser.UserName,
                        Position = await ownScore.Position(_ctx)
                    };

                BeatmapSet set = null;
                
                if (beatmap != null)
                    set = new BeatmapSet
                    {
                        SetID = beatmap.Id,
                        Artist = beatmap.Artist,
                        Title = beatmap.Title,
                        RankedStatus = beatmap.RankedStatus,
                        
                        ChildrenBeatmaps = new List<Beatmap>
                        {
                            new Beatmap
                            {
                                FileMD5 = beatmap.FileMd5,
                                DiffName = beatmap.DiffName,
                                ParentSetID = beatmap.SetId,
                                BeatmapID = beatmap.Id,
                                Mode = beatmap.PlayMode
                            }
                        }
                    };
                
                var sboard = new Scoreboard(set?.ChildrenBeatmaps.FirstOrDefault(bm => bm.FileMD5 == fileMd5),
                                            set, sScores, ownsScore);

                _cache.Set($"sora:Scoreboards:{cacheHash}", cachedData = sboard.ToOsuString(), TimeSpan.FromSeconds(30));
                return Ok(cachedData);
            }
            catch (Exception ex)
            {
                Logger.Err(ex);
                return Ok("Failed");
            }
        }

        #endregion
    }
}
