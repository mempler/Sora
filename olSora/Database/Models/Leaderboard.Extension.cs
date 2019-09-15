using System;
using System.Linq;
using System.Threading.Tasks;
using Sora.Enums;

namespace Sora.Database.Models
{
    public abstract partial class Leaderboard<T> where T : class, ILeaderboard, new()
    {
        public static async Task<T> GetLeaderboardAsync(SoraDbContext ctx, int userId)
        {
            var result = ctx.Set<T>().Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
            if (result != null)
                return result;

            var lb = new T {Id = userId};
            await ctx.Set<T>().AddAsync(lb);
            
            return lb;
        }

        public static async Task<T> GetLeaderboardAsync(SoraDbContext ctx, Users user)
            => await GetLeaderboardAsync(ctx, user.Id);

        public static T GetLeaderboard(SoraDbContext ctx, int userId)
            => GetLeaderboardAsync(ctx, userId).GetAwaiter().GetResult();
        
        public static T GetLeaderboard(SoraDbContext ctx, Users user)
            => GetLeaderboardAsync(ctx, user.Id).GetAwaiter().GetResult();
        
        public void IncreaseScore(ulong score, bool ranked, PlayMode mode)
        {
            switch (mode)
            {
                case PlayMode.Osu:
                    if (ranked)
                        RankedScoreOsu += score;
                    else
                        TotalScoreOsu += score;
                    break;
                
                case PlayMode.Taiko:
                    if (ranked)
                        RankedScoreTaiko += score;
                    else
                        TotalScoreTaiko += score;
                    break;
                
                case PlayMode.Ctb:
                    if (ranked)
                        RankedScoreCtb += score;
                    else
                        TotalScoreCtb += score;
                    break;
                
                case PlayMode.Mania:
                    if (ranked)
                        RankedScoreMania += score;
                    else
                        TotalScoreMania += score;
                    break;
            }
        }
        
        public void IncreasePlaycount(PlayMode mode)
        {
            switch (mode)
            {
                case PlayMode.Osu:
                    PlayCountOsu++;
                    break;
                
                case PlayMode.Taiko:
                    PlayCountTaiko++;
                    break;
                
                case PlayMode.Ctb:
                    PlayCountCtb++;
                    break;
                
                case PlayMode.Mania:
                    PlayCountMania++;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public uint GetPosition(SoraDbContext ctx, PlayMode mode)
        {
            int pos;

            switch (mode)
            {
                case PlayMode.Osu:
                    pos = ctx.Set<T>().Count(x => x.PerformancePointsOsu > PerformancePointsOsu);
                    break;
                
                case PlayMode.Taiko:
                    pos = ctx.Set<T>().Count(x => x.PerformancePointsTaiko > PerformancePointsTaiko);
                    break;
                
                case PlayMode.Ctb:
                    pos = ctx.Set<T>().Count(x => x.PerformancePointsCtb > PerformancePointsCtb);
                    break;
                
                case PlayMode.Mania:
                    pos = ctx.Set<T>().Count(x => x.PerformancePointsMania > PerformancePointsMania);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            return (uint) pos + 1;
        }

        public void SaveChanges(SoraDbContextFactory factory)
        {
            using var db = factory.GetForWrite();
            db.Context.Update((T) (object) this);
        }

        public void SaveChanges(SoraDbContext ctx)
        {
            ctx.Update((T) (object) this);
        }
    }
}
