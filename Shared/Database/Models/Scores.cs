// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Shared.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Enums;
    using Interfaces;

    public class Scores : IOsuStringable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DefaultValue(0)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users ScoreOwner { get; set; }

        [Required]
        public string FileMD5 { get; set; }
        
        [Required]
        public string ScoreMD5 { get; set; }
        
        [Required]
        [DefaultValue("")]
        public string ReplayMD5 { get; set; }
        
        [Required]
        [DefaultValue(0)]
        public int TotalScore { get; set; }

        [Required]
        [DefaultValue(0)]
        public short MaxCombo { get; set; }

        [Required]
        [DefaultValue(PlayMode.Osu)]
        public PlayMode PlayMode { get; set; }

        [Required]
        [DefaultValue(Mod.None)]
        public Mod Mods { get; set; }

        [Required]
        [DefaultValue(0)]
        public short Count300 { get; set; }

        [Required]
        [DefaultValue(0)]
        public short Count100 { get; set; }

        [Required]
        [DefaultValue(0)]
        public short Count50 { get; set; }

        [Required]
        [DefaultValue(0)]
        public short CountMiss { get; set; }

        [Required]
        [DefaultValue(0)]
        public short CountGeki { get; set; }

        [Required]
        [DefaultValue(0)]
        public short CountKatu { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [DefaultValue(0.0f)]
        public double Accuracy { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PeppyPoints { get; set; }

        [NotMapped]
        public int Position { get; set; }


        public string ToOsuString() => $"{Id}|" +
                                       $"{ScoreOwner.Username.Replace("|", "I")}|" +
                                       $"{TotalScore}|" +
                                       $"{MaxCombo}|" +
                                       $"{Count50}|" +
                                       $"{Count100}|" +
                                       $"{Count300}|" +
                                       $"{CountGeki}|" +
                                       $"{CountMiss}|" +
                                       $"{CountKatu}|" +
                                       $"{(CountMiss > 0)}|" +
                                       $"{(short) Mods}|" +
                                       $"{UserId}|" +
                                       $"{Position}|" +
                                       $"{(int) (Date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds}|" +
                                       $"{Convert.ToInt32(ReplayMD5 == string.Empty)}";

        public static IEnumerable<Scores> GetScores(
            string fileMD5, Users user, PlayMode playMode = PlayMode.Osu,
            bool relaxing = false, bool touching = false,
            bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
            Mod mods = Mod.None, bool onlySelf = false)
        {
            using (SoraContext db = new SoraContext())
            {
                CountryIds cid = UserStats.GetUserStats(user.Id).CountryId;

                var query = db.Scores
                                  .Where(score => score.FileMD5 == fileMD5 && score.PlayMode == playMode)
                                  .Where(score
                                      => relaxing
                                          ? (score.Mods & Mod.Relax) != 0
                                          : (score.Mods & Mod.Relax) == 0)
                                  .Where(score
                                      => touching
                                          ? (score.Mods & Mod.TouchDevice) != 0
                                          : (score.Mods & Mod.TouchDevice) == 0)
                                  .Where(score
                                      // ReSharper disable once AccessToDisposedClosure
                                      => !friendsOnly || db.Friends.Where(f => f.UserId == user.Id)
                                                           .Select(f => f.FriendId).Contains(score.UserId))
                                  .Where(score
                                      // ReSharper disable once AccessToDisposedClosure
                                      => !countryOnly || db.UserStats.Select(c => c.CountryId).Contains(cid))
                                  .Where(score => !modOnly || score.Mods == mods)
                                  .Select((s, i) => new {s, i})
                                  .Where(score => !onlySelf || score.s.UserId == user.Id)
                                  .Where(score => setPosition(score.s, score.i))
                                  .Take(50);
                
                return query.Select(score => score.s);
            }
        }

        public static int GetTotalScores(string fileMd5)
        {
            using (SoraContext db = new SoraContext())
                return db.Scores.Count(score => score.FileMD5 == fileMd5);
        }

        private static bool setPosition(Scores s, int pos)
        {
            s.Position = pos;
            return true;
        }
    }
}
