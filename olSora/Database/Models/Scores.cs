#region LICENSE

/*
    olSora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Sora.Enums;

// ReSharper disable AccessToDisposedClosure
namespace Sora.Database.Models
{
    public class Scores
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
        public int Count300 { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count100 { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count50 { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CountMiss { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CountGeki { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CountKatu { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [DefaultValue(0.0f)]
        public double Accuracy { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePoints { get; set; }

        [NotMapped]
        public int Position { get; set; }

        public string ToOsuString(SoraDbContextFactory factory)
        {
            if (ScoreOwner == null)
                ScoreOwner = Users.GetUser(factory, UserId);

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
            SoraDbContextFactory factory,
            string fileMd5, Users user, PlayMode playMode = PlayMode.Osu,
            bool relaxing = false,
            bool friendsOnly = false, bool countryOnly = false, bool modOnly = false,
            Mod mods = Mod.None, bool onlySelf = false)
        {
CountryIds cid = 0;

            if (countryOnly)
                cid = UserStats.GetUserStats(factory, user.Id).CountryId;

            var query = factory.Get().Scores
                               .Where(score => score.FileMd5 == fileMd5 && score.PlayMode == playMode)
                               .Where(
                                   score
                                       => relaxing
                                           ? (score.Mods & Mod.Relax) != 0
                                           : (score.Mods & Mod.Relax) == 0
                               )
                               .Where(
                                   score
                                       => !friendsOnly || factory.Get().Friends
                                                                 .Where(f => f.UserId == user.Id)
                                                                 .Select(f => f.FriendId)
                                                                 .Contains(score.UserId)
                               )
                               .Where(
                                   score
                                       => !countryOnly || factory.Get().UserStats
                                                                 .Select(c => c.CountryId)
                                                                 .Contains(cid)
                               )
                               .Where(score => !modOnly || score.Mods == mods)
                               .Where(score => !onlySelf || score.UserId == user.Id)
                               .OrderByDescending(score => score.TotalScore)
                               .GroupBy(s => s.UserId)
                               .Take(50);

            var result = query.ToArray().Select(s => s.Select(
                xs =>
                {
                    xs.Position = factory.Get()
                                         .Scores
                                         .Where(score => score.FileMd5 == fileMd5 && score.PlayMode == playMode)
                                         .Where(
                                             score
                                                 => relaxing
                                                     ? (score.Mods & Mod.Relax) != 0
                                                     : (score.Mods & Mod.Relax) == 0
                                         )
                                         .Where(
                                             score => (score.Mods & Mod.Relax2) == 0
                                         )
                                         .OrderByDescending(score => score.TotalScore)
                                         .IndexOf(xs) + 1;
                    
                    return xs;
                }).First());

            return result;
        }

        public static int GetTotalScores(SoraDbContextFactory factory, string fileMd5)
        {
            return factory.Get().Scores.Count(score => score.FileMd5 == fileMd5);
        }

        public static Scores GetScore(SoraDbContextFactory factory, int scoreId)
        {
            var s = factory.Get().Scores.First(score => score.Id == scoreId);
            s.ScoreOwner = Users.GetUser(factory, s.UserId);
            return s;
        }

        public static void InsertScore(SoraDbContextFactory factory, Scores score)
        {
            using var db = factory.GetForWrite();

            var sc =
                db.Context.Scores
                  .Where(s => s.FileMd5 == score.FileMd5 && s.PlayMode == score.PlayMode)
                  .Where(
                      s => (score.Mods & Mod.Relax) != 0
                          ? (score.Mods & Mod.Relax) != 0
                          : (score.Mods & Mod.Relax) == 0
                  )
                  .Where(s => s.UserId == score.UserId)
                  .OrderByDescending(s => s.TotalScore);

            var isBetter = sc.Any(scr => scr.TotalScore < score.TotalScore);

            if (isBetter)
                db.Context.RemoveRange(sc);

            db.Context.Add(score);
        }

        public double ComputeAccuracy(bool update = true)
        {
            var totalHits = Count50 + Count100 + Count300 + CountMiss;

            if (PlayMode == PlayMode.Mania)
                totalHits += CountGeki + CountKatu;
            
            var acc = PlayMode switch {
                PlayMode.Osu => totalHits > 0
                    ? (double) (Count50 * 50 + Count100 * 100 + Count300 * 300) /
                      (totalHits * 300)
                    : 1,
                PlayMode.Taiko => totalHits > 0
                    ? (double) (Count100 * 150 + Count300 * 300) /
                      (totalHits * 300)
                    : 1,
                PlayMode.Ctb => totalHits > 0
                    ? (double) (Count50 + Count100 + Count300) /
                      totalHits
                    : 1,
                PlayMode.Mania => totalHits > 0
                    ? (double) (Count50 * 50 + Count100 * 100 + CountKatu * 200 + (Count300 + CountGeki) * 300) /
                      (totalHits * 300)
                    : 1,
                _ => 0
            };

            if (update)
                Accuracy = acc;

            return acc;
        }

        public double ComputePerformancePoints() => PerformancePointsProcessor.Compute(this);
    }
}
