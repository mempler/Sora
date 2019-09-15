using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using osu.Framework.Extensions.IEnumerableExtensions;
using Sora.Database;
using Sora.Database.Models;
using Sora.Helpers;

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
                    s.ComputeAccuracy();
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
                    
                    s.ScoreOwner = Users.GetUser(_factory, s.UserId);

                    try
                    {
                        s.ComputePerformancePoints();
                    } catch (Exception)
                    {
                        // ignored
                    }
                    
                    if (i % 100 == 0)
                        Logger.Info($"Score Id: {s.Id} Performance Points: {s.PerformancePoints:F} Mode {s.PlayMode} +{s.Mods} | {i} of {sCount} {(double) i / (double) sCount:P}");
                }
            );
        }
        
        public void ProcessLowerScores()
        {
            using var db = _factory.GetForWrite();
            var ctx = db.Context;

            var topScores = ctx.Beatmaps
                               .Select(beatmap => ctx.Scores.Where(s => s.FileMd5 == beatmap.FileMd5))
                               .SelectMany(scores => scores)
                               .OrderByDescending(score => score.TotalScore)
                               .GroupBy(s => s.UserId)
                               .Select(s => s.FirstOrDefault())
                               .GroupBy(s => s.FileMd5)
                               .Select(s => s.FirstOrDefault());

            var deletableScores = ctx.Scores
                                     .Where(s => !topScores.Any(sc => sc.Id == s.Id));

            Logger.Info($"Total amount of {topScores.Count()} Top Scores!");
            Logger.Info($"Total amount of {deletableScores.Count()} Deletable Scores!");
            
            ctx.Scores.RemoveRange(deletableScores);
        }
    }
}
