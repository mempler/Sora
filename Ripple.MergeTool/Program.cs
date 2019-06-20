using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Ripple.MergeTool.Database;
using Ripple.MergeTool.Tools;
using Sora.Database;
using Sora.Database.Models;
using Sora.Helpers;

namespace Ripple.MergeTool
{
    internal static class Program
    {
        private static MemoryCache _memoryCache;

        private static void Main()
        {
            Logger.Info("Ripple Merger v0.0.1a");

            if (_memoryCache == null)
                _memoryCache = new MemoryCache(
                    new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromDays(365)}
                );
            var cfgUtil = new ConfigUtil(_memoryCache);

            var cfg =
                cfgUtil.ReadConfig<RippleDbContext.RippleConfig>();

            var factory = new SoraDbContextFactory();
            var rippleCtx = new RippleDbContext();

            #region User Database merge

            using (var db = factory.GetForWrite())
            {
                foreach (var user in rippleCtx.Users)
                {
                    if (db.Context.Users.Any(x => x.Username == user.username)) // Skip double Users
                        continue;

                    if (user.id == 999) // We don't need fokabot! gtfu
                        continue;

                    if (user.email == null || user.password_md5 == null || user.username == null)
                    {
                        Logger.Info($"Invalid User! {user.id}");
                        Thread.Sleep(1000);
                        continue;
                    }

                    Logger.Info($"User: {user.username} ( {user.id} )");

                    db.Context.Users.Add(
                        new Users
                        {
                            Username = user.username,
                            Password = user.password_md5,
                            Permissions = PrivilegeMerger.Merge(user.privileges),
                            Email = user.email
                        }
                    );
                }
            }

            #endregion

            #region Beatmap Databse merge

            using (var db = factory.GetForWrite())
            {
                foreach (var rMap in rippleCtx.Beatmaps)
                {
                    if (db.Context.Beatmaps.Any(x => x.Id == rMap.beatmap_id))
                        continue;

                    Logger.Info($"Importing {rMap.beatmap_id}");

                    Beatmaps sMap;
                    if ((sMap = Beatmaps.FetchFromApi(cfg, null, rMap.beatmap_id)) != null)
                    {
                        sMap.RankedStatus = rMap.ranked;

                        var LetsMapPath = Path.Join(cfg.CRipple.LetsBeatmapPath, "/" + sMap.Id + ".osu");
                        var SoraMapPath = Path.Join(cfg.SoraDataDirectory, "/beatmaps/" + sMap.FileMd5);

                        Logger.Info(LetsMapPath);
                        Logger.Info(SoraMapPath);

                        if (File.Exists(LetsMapPath) && !File.Exists(SoraMapPath))
                            File.Copy(LetsMapPath, SoraMapPath);
                        else
                            BeatmapDownloader.GetBeatmap(sMap.FileMd5, cfg);

                        if (db.Context.Beatmaps.Count(x => x.Id == sMap.Id) < 1)
                            db.Context.Beatmaps.Add(sMap);
                        else
                            db.Context.Beatmaps.Update(sMap);
                    }
                }
            }

            #endregion

            #region Score Database Merge.

            foreach (var score in rippleCtx.Scores)
            {
                var soraScore = new Scores();
            }

            #endregion

            Logger.Info("Done! Requires manual Checking...");
            Thread.Sleep(5000);
        }
    }
}
