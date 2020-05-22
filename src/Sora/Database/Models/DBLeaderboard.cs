using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Extensions.IEnumerableExtensions;
using Sora.Framework.Enums;

namespace Sora.Database.Models
{
    [Table("Leaderboard")]
    public class DbLeaderboard
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int OwnerId { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreMania { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreMania { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountMania { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsMania { get; set; }

        [Required]
        [UsedImplicitly]
        [ForeignKey(nameof(OwnerId))]
        public DbUser Owner { get; set; }
        
        public static async Task<DbLeaderboard> GetLeaderboardAsync(SoraDbContext ctx, int userId)
        {
            var result = ctx.Leaderboard.Where(t => t.OwnerId == userId).Select(e => e).AsNoTracking().FirstOrDefault();
            if (result != null)
                return result;

            var lb = new DbLeaderboard{OwnerId = userId};
            
            await ctx.Leaderboard.AddAsync(lb);
            await ctx.SaveChangesAsync();
            
            return lb;
        }

        public static async Task<DbLeaderboard> GetLeaderboardAsync(SoraDbContext ctx, DbUser user)
            => await GetLeaderboardAsync(ctx, user.Id);

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
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
            var pos = mode switch // Prevent position jumping around.
            {
                PlayMode.Osu => PerformancePointsOsu <= 0 ? 0 : -1,
                PlayMode.Taiko => PerformancePointsTaiko <= 0 ? 0 : -1,
                PlayMode.Ctb => PerformancePointsCtb <= 0 ? 0 : -1,
                PlayMode.Mania => PerformancePointsMania <= 0 ? 0 : -1,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            if (pos == -1)
                pos = mode switch
                {
                    PlayMode.Osu => ctx.Leaderboard.Count(x => x.PerformancePointsOsu > PerformancePointsOsu),
                    PlayMode.Taiko => ctx.Leaderboard
                                         .Count(x => x.PerformancePointsTaiko > PerformancePointsTaiko),
                    PlayMode.Ctb => ctx.Leaderboard.Count(x => x.PerformancePointsCtb > PerformancePointsCtb),
                    PlayMode.Mania => ctx.Leaderboard
                                         .Count(x => x.PerformancePointsMania > PerformancePointsMania),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
            else
                return (uint) pos;
            
            return (uint) pos + 1;
        }

        public double GetAccuracy(SoraDbContext ctx, PlayMode mode)
        {
            var totalAcc = 0d;
            var divideTotal = 0d;
            var i = 0;

            ctx
                .Scores
                .Where(s => s.PlayMode == mode)
               //.Where(s => (s.Mods & Mod.Relax) == 0 && (s.Mods & Mod.Relax2) == 0)
                .Where(s => s.UserId == OwnerId)
                .Take(500)
                .OrderByDescending(s => s.PerformancePoints)
                .ForEach(
                    s =>
                    {
                        var divide = Math.Pow(.95d, i);

                        totalAcc += s.Accuracy * divide;
                        divideTotal += divide;

                        i++;
                    }
                );

            return divideTotal > 0 ? totalAcc / divideTotal : 0;
        }

        // TODO: optimize this
        public void UpdatePp(SoraDbContext ctx, PlayMode mode)
        {
            var totalPp = ctx
                                 .Scores
                                 //.Where(s => (s.Mods & Mod.Relax) == 0)
                                 .Where(s => s.PlayMode == mode)
                                 .Where(s => s.UserId == OwnerId)
                                 .OrderByDescending(s => s.PerformancePoints)
                                 .ToList() // There goes our memory one more time :c
                                 .GroupBy(s => s.FileMd5)
                                 .Select(s => s.First())
                                 .Take(100)
                                 .Select((t, i) => t.PerformancePoints * Math.Pow(0.95d, i))
                                 .Sum();
            switch (mode)
            {
                case PlayMode.Osu:
                    PerformancePointsOsu = totalPp;
                    break;

                case PlayMode.Taiko:
                    PerformancePointsTaiko = totalPp;
                    break;

                case PlayMode.Ctb:
                    PerformancePointsCtb = totalPp;
                    break;

                case PlayMode.Mania:
                    PerformancePointsMania = totalPp;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public async Task SaveChanges(SoraDbContext ctx)
        {
            ctx.Update(this);
            
            await ctx.SaveChangesAsync();
        }
    }
}