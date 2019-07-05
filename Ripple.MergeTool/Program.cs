using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Amib.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Ripple.MergeTool.Database;
using Ripple.MergeTool.Tools;
using Sora;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.Helpers;

namespace Ripple.MergeTool
{
    internal static class Program
    {
        private static MemoryCache _memoryCache;
        private static SmartThreadPool _pool;
        private static void Main()
        {
            Logger.Info("Ripple Merger v0.0.1a");

            _memoryCache = new MemoryCache(
                new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromDays(365)}
            );

            _pool = new SmartThreadPool {MaxThreads = Environment.ProcessorCount * 2};

            _pool.Start();
            
            var cfgUtil = new ConfigUtil(_memoryCache);

            var cfg = cfgUtil.ReadConfig<RippleDbContext.RippleConfig>();

            var factory = new SoraDbContextFactory();
            var rippleCtx = new RippleDbContext();

            var BoundUsers = new List<Tuple<Users, int>>();

            #region User Table Merge

            Logger.Info("Begin Merging of Users");
            foreach (var user in rippleCtx.Users.Where(u => u.id != 999))
            {
                if (factory.Get().Users.Any(x => x.Username == user.username)) // Skip double Users
                {
                    var u1 = factory.Get().Users.FirstOrDefault(u => u.Username == user.username);
                    if (u1 == null)
                        continue;

                    BoundUsers.Add(Tuple.Create(u1, user.id));
                    continue;
                }

                if (user.email == null || user.password_md5 == null || user.username == null)
                {
                    Logger.Info($"Invalid User! {user.id}");
                    Thread.Sleep(1000);
                    continue;
                }
                
                using (var db = factory.GetForWrite())
                    db.Context.Users.Add(
                        new Users
                        {
                            Username = user.username,
                            Password = user.password_md5,
                            Permissions = PrivilegeMerger.Merge(user.privileges),
                            Email = user.email
                        }
                    );

                
                var u2 = factory.Get().Users.FirstOrDefault(u => u.Username == user.username);
                if (u2 == null)
                    continue;

                BoundUsers.Add(Tuple.Create(u2, user.id));
            }
            
            #endregion

            #region Beatmap Table Merge
            Logger.Info("Begin Merging of Beatmaps");
            
            foreach (var rMap in rippleCtx.Beatmaps.Where(m =>
                m.ranked == RankedStatus.Approved ||
                m.ranked == RankedStatus.Ranked
            )) _pool.QueueWorkItem(rippleMap => {
                if (factory.Get().Beatmaps.Any(x => x.Id == rippleMap.beatmap_id))
                    return;

                Beatmaps sMap;
                if ((sMap = Beatmaps.FetchFromApi(cfg, null, rippleMap.beatmap_id)) == null)
                    return;
                
                sMap.RankedStatus = rippleMap.ranked;
                
                var LetsMapPath = Path.Join(cfg.CRipple.LetsBeatmapPath, "/" + sMap.Id + ".osu");
                var SoraMapPath = Path.Join(cfg.SoraDataDirectory, "/beatmaps/" + sMap.FileMd5);

                if (File.Exists(LetsMapPath) && !File.Exists(SoraMapPath))
                    File.Copy(LetsMapPath, SoraMapPath);

                if (factory.Get().Beatmaps.Count(x => x.Id == sMap.Id) < 1)
                    using (var db = factory.GetForWrite())
                        db.Context.Beatmaps.Add(sMap);
                else
                    using (var db = factory.GetForWrite())
                        db.Context.Beatmaps.Update(sMap);
            }, rMap);
            _pool.WaitForIdle();
            #endregion
            
            #region Score Database Merge
            Logger.Info("Begin Merging of Scores");

            var bmLock = new object();
            foreach (var rDBScore in rippleCtx.Scores)
            {
                _pool.QueueWorkItem(
                    (rScore, boundUsers) =>
                    {
                        var scoreDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        scoreDate = double.TryParse(rScore.time, out var i) ? scoreDate.AddSeconds(i).ToUniversalTime() : DateTime.Now;
                        
                        var soraScore = new Scores
                        {
                            Accuracy = rScore.accuracy,
                            Count300 = rScore.C300,
                            Count100 = rScore.C100,
                            Count50 = rScore.C50,
                            CountGeki = rScore.CGeki,
                            CountKatu = rScore.CKatu,
                            CountMiss = rScore.CMiss,
                            Date = scoreDate,
                            FileMd5 = rScore.beatmap_md5,
                            PlayMode = (PlayMode) rScore.play_mode,
                            Mods = (Mod) rScore.mods,
                            MaxCombo = (short) rScore.max_combo,
                            TotalScore = rScore.score
                        };

                        soraScore.ScoreMd5 = Hex.ToHex(
                            Crypto.GetMd5(
                                $"{soraScore.Count300 + soraScore.Count100}{soraScore.FileMd5}{soraScore.CountMiss}{soraScore.CountGeki}{soraScore.CountKatu}{soraScore.Date}{soraScore.Mods}"
                            )
                        );

                        // Prevent Multiple Records
                        if (factory.Get().Scores.Any(s => s.ScoreMd5 == soraScore.ScoreMd5))
                            return;

                        var user = boundUsers.FirstOrDefault(use => use.Item2 == rScore.userid);

                        if (user == null) // Deleted user ?
                        {
                            Logger.Info($"User for UserId {rScore.userid} not found! Skipping Score...");
                            return;
                        }

                        soraScore.UserId = user.Item1.Id;
                        soraScore.ScoreOwner = user.Item1;

                        var ReplaySoraPath = string.Empty;
                        var ReplayRipplePath = Path.Join(cfg.CRipple.LetsReplaysPath, "replay_" + rScore.id + ".osr");

                        if (!Directory.Exists(Path.Join(cfg.SoraDataDirectory, "/replays/")))
                            Directory.CreateDirectory(Path.Join(cfg.SoraDataDirectory, "/replays/"));

                        if (File.Exists(ReplayRipplePath))
                        {
                            using (var f = File.OpenRead(ReplayRipplePath))
                                soraScore.ReplayMd5 = Hex.ToHex(Crypto.GetMd5(f));
                            
                            ReplaySoraPath = Path.Join(cfg.SoraDataDirectory, "/replays/", soraScore.ReplayMd5);
                            
                            if (!File.Exists(ReplaySoraPath))
                                File.Copy(ReplayRipplePath, ReplaySoraPath);
                        }

                        var WorkingBeatmap = factory.Get().Beatmaps.FirstOrDefault(
                            bm => bm.RankedStatus == RankedStatus.Approved ||
                                  bm.RankedStatus == RankedStatus.Ranked
                                  && bm.FileMd5 == soraScore.FileMd5
                        );
                        
                        if (WorkingBeatmap != null) {
                            
                            var SoraMapPath = Path.Join(cfg.SoraDataDirectory, "/beatmaps/" + soraScore.FileMd5);
                            try {
                                lock (bmLock)
                                    if (!File.Exists(SoraMapPath))
                                        BeatmapDownloader.GetBeatmap(soraScore.FileMd5, cfg, Path.Join(cfg.SoraDataDirectory, "/beatmaps/"));
                            } catch (Exception ex)
                            {
                                Logger.Err(ex);
                            }

                            lock (bmLock)
                                if (!File.Exists(SoraMapPath))
                                {
                                    Logger.Info($"Map {soraScore.FileMd5} Doesn't Exists! Skipping...");
                                    return;
                                }

                            try
                            {
                                if (!string.IsNullOrEmpty(ReplaySoraPath))
                                    soraScore.PeppyPoints = PerformancePointsProcessor.Compute(
                                        soraScore, ReplaySoraPath, SoraMapPath
                                    );
                            } catch (Exception ex)
                            {
                                Logger.Err(ex);
                                Logger.Info($"Failed to Calculate PP for score {rScore.id}");
                            }
                        }

                        if (soraScore.ReplayMd5 == null)
                            soraScore.ReplayMd5 = " ";
                        
                        using (var db = factory.GetForWrite())
                            db.Context.Scores.Add(soraScore);
                    }, rDBScore, BoundUsers
                );
            }
            _pool.WaitForIdle();
            #endregion

            #region Friend Database Merge

            Logger.Info("Begin Merging of Friends");

            foreach (var friend in rippleCtx.Friends)
            {
                using (var db = factory.GetForWrite())
                    db.Context.Friends.Add(new Friends
                    {
                        UserId = friend.user1,
                        FriendId = friend.user2
                    });
            }

            #endregion

            #region Copy Screenshots
            if (Directory.Exists(cfg.CRipple.LetsScreenshotPath))
            {
                var screenshotPath = Path.Join(cfg.SoraDataDirectory, "screenshots");
                if (!Directory.Exists(screenshotPath))
                    Directory.CreateDirectory(screenshotPath);

                foreach (var f in Directory.GetFiles(cfg.CRipple.LetsScreenshotPath).Where(f => !File.Exists(f)))
                {
                    File.Copy(f, Path.Join(screenshotPath, Path.GetFileName(f)));
                }
            }
            #endregion

            Logger.Info("Done! Requires manual Checking...");
            Thread.Sleep(5000);
        }
    }
}
