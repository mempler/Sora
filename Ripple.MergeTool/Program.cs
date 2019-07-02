using System;
using System.IO;
using System.Linq;
using System.Threading;
using Amib.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ripple.MergeTool.Database;
using Ripple.MergeTool.Tools;
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

            #region User Database merge

            using (var db = factory.GetForWrite())
            {
                foreach (var user in rippleCtx.Users.Where(u => u.id != 999))
                {
                    if (db.Context.Users.Any(x => x.Username == user.username)) // Skip double Users
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
            foreach (var rMap in rippleCtx.Beatmaps.Where(m =>
                m.ranked == RankedStatus.Approved ||
                m.ranked == RankedStatus.Ranked
            )) _pool.QueueWorkItem(rippleMap => {
                if (factory.Get().Beatmaps.Any(x => x.Id == rippleMap.beatmap_id))
                    return;

                Beatmaps sMap;
                if ((sMap = Beatmaps.FetchFromApi(cfg, null, rippleMap.beatmap_id)) == null)
                    return;

                Logger.Info($"Importing {rippleMap.beatmap_id}");
                
                sMap.RankedStatus = rippleMap.ranked;
                
                var LetsMapPath = Path.Join(cfg.CRipple.LetsBeatmapPath, "/" + sMap.Id + ".osu");
                var SoraMapPath = Path.Join(cfg.SoraDataDirectory, "/beatmaps/" + sMap.FileMd5);
                
                Logger.Debug(LetsMapPath);
                Logger.Debug(SoraMapPath);
                
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
