using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Ripple.MergeTool.Database;
using Ripple.MergeTool.Database.Model;
using Ripple.MergeTool.Tools;
using Sora.Database;
using Sora.Database.Models;
using Sora.Helpers;

namespace Ripple.MergeTool
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Logger.Info("Ripple Merger v0.0.1a");

            RippleDbContext.RippleConfig cfg = ConfigUtil.ReadConfig<RippleDbContext.RippleConfig>("ripple.config.json");
            SoraDbContextFactory factory = new SoraDbContextFactory();
            RippleDbContext rippleCtx = new RippleDbContext();
            
            using DatabaseWriteUsage db = factory.GetForWrite();
            
            #region User Database merge
            foreach (RippleUser user in rippleCtx.Users)
            {
                if (user.id == 999) // We don't need fokabot! gtfu
                    continue;

                if (user.email == null || user.password_md5 == null || user.username == null)
                {
                    Logger.Info($"Invalid User! {user.id}");
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Info($"User: {user.username} ( {user.id} )");

                db.Context.Users.Add(new Users
                {
                    Username   = user.username,
                    Password   = user.password_md5,
                    Privileges = PrivilegeMerger.Merge(user.privileges),
                    Email      = user.email
                });
            }
            #endregion
            
            #region Beatmap Databse merge
            foreach (RippleBeatmap rMap in rippleCtx.Beatmaps)
            {
                Logger.Info($"Importing {rMap.beatmap_id}");
                
                Beatmaps sMap;
                if ((sMap = Beatmaps.FetchFromApi(cfg, null, rMap.beatmap_id)) != null)
                {
                    sMap.RankedStatus = rMap.ranked;
                    Logger.Info(cfg.CRipple.LetsBeatmapPath + "/" + sMap.Id + ".osu");
                    Logger.Info(cfg.SoraDataDirectory + "/beatmaps/" + sMap.FileMd5);

                    if (File.Exists(cfg.CRipple.LetsBeatmapPath + "/" + sMap.Id + ".osu") && !File.Exists(
                            cfg.SoraDataDirectory + "/beatmaps/" + sMap.FileMd5))
                        File.Copy(cfg.CRipple.LetsBeatmapPath + "/" + sMap.Id + ".osu",
                                  cfg.SoraDataDirectory + "/beatmaps/" + sMap.FileMd5);
                    else
                        BeatmapDownloader.GetBeatmap(sMap.FileMd5, cfg);
                    
                    if (db.Context.Beatmaps.Count(x => x.Id == sMap.Id) < 1)
                        db.Context.Beatmaps.Add(sMap);
                    else
                        db.Context.Beatmaps.Update(sMap);
                }
            }
            #endregion
            
            #region Score Database Merge.
            foreach (RippleScore score in rippleCtx.Scores)
            {
                Scores soraScore = new Scores
                {

                };
            }
            #endregion

            Logger.Info("Done! Requires manual Checking...");
            Thread.Sleep(5000);
        }
    }
}