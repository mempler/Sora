using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using osu.Framework.Graphics.Containers;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Scores;
using Sora.Framework.Utilities;

namespace Sora.Database.Models
{
    [Table("Scores")]
    public class DBScore
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }

        public int UserId { get; set; }

        public string FileMd5 { get; set; }
        public int Count300 { get; set; }
        public int Count100 { get; set; }
        public int Count50 { get; set; }
        public int CountGeki { get; set; }
        public int CountKatu { get; set; }
        public int CountMiss { get; set; }
        public int TotalScore { get; set; }
        public short MaxCombo { get; set; }
        public Mod Mods { get; set; }
        public PlayMode PlayMode { get; set; }
        public DateTime Date { get; set; }

        public string? ReplayMd5 { get; set; }
        public double Accuracy { get; set; }
        public double PerformancePoints { get; set; }
        
        [Required]
        [ForeignKey(nameof(UserId))]
        public DBUser ScoreOwner { get; set; }

        public async Task<int> Position(SoraDbContextFactory factory)
            => await factory.Get().Scores.Where(s => s.TotalScore > TotalScore &&
                                                     s.FileMd5 == FileMd5 &&
                                                     s.PlayMode == PlayMode &&
                                                     s.UserId != UserId).CountAsync() +1;

        public double ComputeAccuracy()
        {
            var totalHits = Count50 + Count100 + Count300 + CountMiss;

            if (PlayMode == PlayMode.Mania)
                totalHits += CountGeki + CountKatu;

            return PlayMode switch
            {
                PlayMode.Osu => (totalHits > 0
                    ? (double) (Count50 * 50 + Count100 * 100 + Count300 * 300) / (totalHits * 300)
                    : 1),
                PlayMode.Taiko => (totalHits > 0 ? (double) (Count100 * 150 + Count300 * 300) / (totalHits * 300) : 1),
                PlayMode.Ctb => (totalHits > 0 ? (double) (Count50 + Count100 + Count300) / totalHits : 1),
                PlayMode.Mania => (totalHits > 0
                    ? (double) (Count50 * 50 + Count100 * 100 + CountKatu * 200 + (Count300 + CountGeki) * 300) /
                      (totalHits * 300)
                    : 1),
                _ => 0
            };
        }

        public double ComputePerformancePoints() => PerformancePointsProcessor.Compute(this);

        public string Checksum => Hex.ToHex(
            Crypto.GetMd5(
                $"{Count300 + Count100}{FileMd5}{CountMiss}{CountGeki}{CountKatu}{Date}{Mods}"
            )
        );

        public static async Task<List<DBScore>> GetScores(
                SoraDbContextFactory factory,
                string fileMd5, DBUser user, PlayMode playMode = PlayMode.Osu,
                bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
                Mod mods = Mod.None, bool onlySelf = false
            )
        {
            const CountryId cid = CountryId.XX;

            /* Legacy Code
            if (countryOnly)
                cid = UserStats.GetUserStats(factory, user.Id).CountryId;
            */

            var query = factory.Get().Scores
                               .Where(score => score.FileMd5 == fileMd5 && score.PlayMode == playMode)
                               .Where(
                                   score
                                       => !friendsOnly || factory.Get().Friends
                                                                 .Where(f => f.UserId == user.Id)
                                                                 .Select(f => f.FriendId)
                                                                 .Contains(score.UserId)
                               )
                               /* Legacy Code TODO: Implement
                               .Where(
                                   score
                                       => !countryOnly || factory.Get().UserStats
                                                                 .Select(c => c.CountryId)
                                                                 .Contains(cid)
                               )
                               */
                               .Where(score => !modOnly || score.Mods == mods)
                               .Where(score => !onlySelf || score.UserId == user.Id)
                               .OrderByDescending(score => score.TotalScore)
                               .ThenByDescending(s => s.Accuracy) // Order by TotalScore and Accuracy. Score V2 Support
                               .Include(x => x.ScoreOwner)
                               .ToList() // There goes our memory :c
                               .GroupBy(s => s.UserId) // Requires to be Client Side since EF Core is the big stupid and don't recognize this.
                               .Take(50)
                               .Select(s => s.First());

            return query.ToList();
        }

        public static Task<DBScore> GetScore(
            SoraDbContextFactory factory,
            int replayId
        ) => factory.Get().Scores.Where(x => x.Id == replayId).FirstOrDefaultAsync();

        public static Task<DBScore> GetScore(
            SoraDbContextFactory factory,
            string fileMd5
        ) => factory.Get().Scores.Where(x => x.FileMd5 == fileMd5).FirstOrDefaultAsync();

        public static Task<DBScore> GetScore(
            SoraDbContextFactory factory,
            DBScore otherScore
        ) => factory.Get().Scores.Where(x => x.Checksum == otherScore.Checksum).FirstOrDefaultAsync();

        public static Task<DBScore> GetLatestScore(
            SoraDbContextFactory factory,
            DBScore NewerScore
        ) => factory.Get().Scores.Where(x => x.FileMd5 == NewerScore.FileMd5 &&
                                             x.UserId == NewerScore.UserId &&
                                             x.PlayMode == NewerScore.PlayMode &&
                                             x.TotalScore >= NewerScore.TotalScore)
                    .OrderByDescending(x => x.TotalScore).ThenByDescending(s => s.Accuracy)
                    .FirstOrDefaultAsync();

        public static void InsertScore(SoraDbContextFactory factory, DBScore score)
        {
            var so = score.ScoreOwner; // Fix for an Issue with Inserting.
            
            using (var db = factory.GetForWrite())
            {
                score.ScoreOwner = null;
                db.Context.Add(score);
            }
            
            score.ScoreOwner = so; // Restore ScoreOwner
        }
    }
}