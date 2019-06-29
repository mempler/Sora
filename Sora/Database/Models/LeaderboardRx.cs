#region LICENSE

/*
    Sora - A Modular Bancho written in C#
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

// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sora.Enums;
using Sora.Helpers;

namespace Sora.Database.Models
{
    public class LeaderboardRx
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
        public ulong TotalScoreOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count300Osu { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count300Taiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count300Ctb { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count100Osu { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count100Taiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count100Ctb { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count50Osu { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count50Taiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public int Count50Ctb { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CountMissOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CountMissTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public int CountMissCtb { get; set; }

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
        public double PerformancePointsOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsCtb { get; set; }

        public static LeaderboardRx GetLeaderboard(SoraDbContextFactory factory, int userId)
        {
            using var db = factory.GetForWrite();

            var result = db.Context.LeaderboardRx.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
            if (result != null)
                return result;

            db.Context.LeaderboardRx.Add(new LeaderboardRx {Id = userId});

            return new LeaderboardRx {Id = userId};
        }

        public double GetAccuracy(PlayMode mode)
        {
            switch (mode)
            {
                case PlayMode.Osu:
                    return Accuracy.GetAccuracy(Count300Osu, Count100Osu, Count50Osu, CountMissOsu, 0, 0, PlayMode.Osu);
                case PlayMode.Taiko:
                    return Accuracy.GetAccuracy(
                        Count300Taiko, Count100Taiko, Count50Taiko, CountMissTaiko, 0, 0, PlayMode.Osu
                    );
                case PlayMode.Ctb:
                    return Accuracy.GetAccuracy(Count300Ctb, Count100Ctb, Count50Ctb, CountMissCtb, 0, 0, PlayMode.Osu);
            }

            return 0;
        }
        public void IncreaseScore(SoraDbContextFactory factory, ulong score, bool ranked, PlayMode mode)
        {
            using var db = factory.GetForWrite();

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
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public void IncreaseCount300(SoraDbContextFactory factory, int c, PlayMode mode)
        {
            using var db = factory.GetForWrite();

            switch (mode)
            {
                case PlayMode.Osu:
                    Count300Osu += c;
                    break;
                case PlayMode.Taiko:
                    Count300Taiko += c;
                    break;
                case PlayMode.Ctb:
                    Count300Ctb += c;
                    break;
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public void IncreaseCount100(SoraDbContextFactory factory, int c, PlayMode mode)
        {
            using var db = factory.GetForWrite();

            switch (mode)
            {
                case PlayMode.Osu:
                    Count100Osu += c;
                    break;
                case PlayMode.Taiko:
                    Count100Taiko += c;
                    break;
                case PlayMode.Ctb:
                    Count100Ctb += c;
                    break;
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public void IncreaseCount50(SoraDbContextFactory factory, int c, PlayMode mode)
        {
            using var db = factory.GetForWrite();

            switch (mode)
            {
                case PlayMode.Osu:
                    Count50Osu += c;
                    break;
                case PlayMode.Taiko:
                    Count50Taiko += c;
                    break;
                case PlayMode.Ctb:
                    Count50Ctb += c;
                    break;
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public void IncreaseCountMiss(SoraDbContextFactory factory, int c, PlayMode mode)
        {
            using var db = factory.GetForWrite();

            switch (mode)
            {
                case PlayMode.Osu:
                    CountMissOsu += c;
                    break;
                case PlayMode.Taiko:
                    CountMissTaiko += c;
                    break;
                case PlayMode.Ctb:
                    CountMissCtb += c;
                    break;
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public void IncreasePlaycount(SoraDbContextFactory factory, PlayMode mode)
        {
            using var db = factory.GetForWrite();

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
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public void UpdatePP(SoraDbContextFactory factory, PlayMode mode)
        {
            using var db = factory.GetForWrite();

            var TotalPP = db.Context.Scores
                            .Where(s => (s.Mods & Mod.Relax) == 0)
                            .Where(s => s.PlayMode == mode)
                            .Where(s => s.UserId == Id)
                            .OrderByDescending(s => s.PeppyPoints)
                            .Take(100).ToList()
                            .Select((t, i) => t.PeppyPoints * Math.Pow(0.95d, i))
                            .Sum();

            switch (mode)
            {
                case PlayMode.Osu:
                    PerformancePointsOsu = TotalPP;
                    break;
                case PlayMode.Taiko:
                    PerformancePointsTaiko = TotalPP;
                    break;
                case PlayMode.Ctb:
                    PerformancePointsCtb = TotalPP;
                    break;
                case PlayMode.Mania:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            db.Context.LeaderboardRx.Update(this);
        }

        public uint GetPosition(SoraDbContextFactory factory, PlayMode mode)
        {
            var pos = 0;

            switch (mode)
            {
                case PlayMode.Osu:
                    pos = factory.Get().LeaderboardRx.Count(x => x.PerformancePointsOsu > PerformancePointsOsu);
                    break;
                case PlayMode.Taiko:
                    pos = factory.Get().LeaderboardRx.Count(x => x.PerformancePointsTaiko > PerformancePointsTaiko);
                    break;
                case PlayMode.Ctb:
                    pos = factory.Get().LeaderboardRx.Count(x => x.PerformancePointsCtb > PerformancePointsCtb);
                    break;
            }

            return (uint) pos;
        }

        public static LeaderboardRx GetLeaderboard(SoraDbContextFactory factory, Users user)
            => GetLeaderboard(factory, user.Id);
    }
}
