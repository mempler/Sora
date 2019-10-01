using System;
using System.IO;
using System.Linq;
using osu.Framework.Extensions.IEnumerableExtensions;
using Sora.Database;
using Sora.Framework.Utilities;

namespace Sora
{
    public class RecalculationProcessor
    {
        private readonly SoraDbContextFactory _factory;

        public RecalculationProcessor(SoraDbContextFactory factory) => _factory = factory;

        public void ProcessAccuracyRecalculation()
        {
            using var db = _factory.GetForWrite();
            var ctx = db.Context;

            var scores = ctx.Users
                            .Select(user => ctx.Scores.Where(s => s.UserId == user.Id))
                            .SelectMany(sc => sc);

            var i = 0;
            var sCount = scores.Count();

            scores.ForEach(
                s =>
                {
                    i++;
                    s.Accuracy = s.ComputeAccuracy();
                    if (i % 10000 == 0)
                        Logger.Info($"Score Id: {s.Id} | {i} of {sCount} {(double)i/(double)sCount:P}");
                });
        }

        public void ProcessPerformanceRecalculation()
        {
            using var db = _factory.GetForWrite();
            var ctx = db.Context;

            var scores = ctx.Users
                            .Select(user => ctx.Scores.Where(s => s.UserId == user.Id))
                            .SelectMany(s => s);

            var i = 0;
            var sCount = scores.Count();

            scores.AsParallel().ForEach(
                s =>
                {
                    i++;
                    if (!File.Exists("data/beatmaps/" + s.FileMd5))
                    {
                        s.PerformancePoints = 0;
                        return;
                    }

                    if (!File.Exists("data/replays/" + s.ReplayMd5))
                    {
                        s.PerformancePoints = 0;
                        return;
                    }
                    
                    try
                    {
                        s.PerformancePoints = s.ComputePerformancePoints();
                    } catch (Exception)
                    {
                        // ignored
                    }
                    
                    if (i % 100 == 0)
                        Logger.Info($"Score Id: {s.Id} Performance Points: {s.PerformancePoints:F} Mode {s.PlayMode} +{s.Mods} | {i} of {sCount} {(double) i / (double) sCount:P}");
                }
            );
        }
    }
}
