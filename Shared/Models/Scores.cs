// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Shared.Enums;
using Shared.Interfaces;
using Shared.Services;
using PlayMode = Shared.Enums.PlayMode;
// ReSharper disable AccessToDisposedClosure

namespace Shared.Models
{
    public class Scores : IOsuStringable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DefaultValue(0)]
        public int UserId { get; set; }

        [NotMapped]
        public Users ScoreOwner { get; set; }

        [Required]
        public string FileMd5 { get; set; }

        [Required]
        public string ScoreMd5 { get; set; }

        [Required]
        [DefaultValue("")]
        public string ReplayMd5 { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScore { get; set; }

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
        public ulong Count300 { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong Count100 { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong Count50 { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong CountMiss { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong CountGeki { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong CountKatu { get; set; }

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


        public string ToOsuString(Database db)
        {
            if (db == null)
                throw new Exception();
            
            if (ScoreOwner == null)
                ScoreOwner = Users.GetUser(db, UserId);
            
            return $"{Id}|" +
                   $"{ScoreOwner.Username.Replace("|", "I")}|" +
                   $"{TotalScore}|" +
                   $"{MaxCombo}|" +
                   $"{Count50}|" +
                   $"{Count100}|" +
                   $"{Count300}|" +
                   $"{CountMiss}|" +
                   $"{CountGeki}|" +
                   $"{CountKatu}|" +
                   $"{CountMiss > 0}|" +
                   $"{(short) Mods}|" +
                   $"{UserId}|" +
                   $"{Position}|" +
                   $"{(int) Date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds}|" +
                   $"{Convert.ToInt32(ReplayMd5 != string.Empty)}";
        }

        public static IEnumerable<Scores> GetScores(
            Database db,
            
            string fileMd5, Users user, PlayMode playMode = PlayMode.Osu,
            bool relaxing = false, bool touching = false,
            bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
            Mod mods = Mod.None, bool onlySelf = false)
        {
            IEnumerable<Scores> result;
            CountryIds cid = 0;
            if (countryOnly)
                cid = UserStats.GetUserStats(db, user.Id).CountryId;

            IQueryable<Scores> query = db.Scores
                                         .Where(score => score.FileMd5 == fileMd5 && score.PlayMode == playMode)
                                         .Where(score
                                                    => relaxing
                                                        ? (score.Mods & Mod.Relax) != 0
                                                        : (score.Mods & Mod.Relax) == 0)
                                         .Where(score
                                                    => touching
                                                        ? (score.Mods & Mod.TouchDevice) != 0
                                                        : (score.Mods & Mod.TouchDevice) == 0)
                                         .Where(score
                                                    => !friendsOnly || db.Friends.Where(f => f.UserId == user.Id)
                                                                         .Select(f => f.FriendId)
                                                                         .Contains(score.UserId))
                                         .Where(score
                                                    => !countryOnly ||
                                                       db.UserStats.Select(c => c.CountryId).Contains(cid))
                                         .Where(score => !modOnly || score.Mods == mods)
                                         //.Select((s, i) => new {s, i})
                                         .Where(score => !onlySelf || score.UserId == user.Id)
                                         //.Where(score => setPosition(score.s, score.i))
                                         .Take(50);

            
            
            result = query.ToArray();
            foreach (Scores s in result)
            {
                // inefficient but it works.
                int selfPosition = db.Scores
                                     .Where(score => score.FileMd5 == fileMd5 && score.PlayMode == playMode)
                                     .Where(score
                                                => relaxing
                                                    ? (score.Mods & Mod.Relax) != 0
                                                    : (score.Mods & Mod.Relax) == 0)
                                     .Where(score
                                                => touching
                                                    ? (score.Mods & Mod.TouchDevice) != 0
                                                    : (score.Mods & Mod.TouchDevice) == 0)
                                     .IndexOf(s);
                s.Position = selfPosition;
            }
            return result;
        }

        public static int GetTotalScores(Database db, string fileMd5) =>
            db.Scores.Count(score => score.FileMd5 == fileMd5);

        public static Scores GetScore(Database db, int scoreId) =>
            db.Scores.First(score => score.Id == scoreId);

        public static void InsertScore(Database db, Scores score)
        {
            db.Add(score);
            db.SaveChanges();
        }
    }
}