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

// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using osu.Framework.Extensions.IEnumerableExtensions;
using Sora.Enums;

namespace Sora.Database.Models
{
    [Table("LeaderboardStd")]
    public class LeaderboardStd : Leaderboard<LeaderboardStd>
    {
        public override double GetAccuracy(SoraDbContext ctx, PlayMode mode)
        {
            var totalAcc = 0d;
            var divideTotal = 0d;
            var i = 0;

            ctx
                .Scores
                .Where(s => s.PlayMode == mode)
                .Where(s => (s.Mods & Mod.Relax) == 0 && (s.Mods & Mod.Relax2) == 0)
                .Where(s => s.UserId == Id)
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

        public override void UpdatePP(SoraDbContext ctx, PlayMode mode)
        {
            var TotalPP = ctx
                          .Scores
                          .Where(s => (s.Mods & Mod.Relax) == 0)
                          .Where(s => s.PlayMode == mode)
                          .Where(s => s.UserId == Id)
                          .OrderByDescending(s => s.PerformancePoints)
                          .Take(100)
                          .ToList()
                          .AsParallel()
                          .Select((t, i) => t.PerformancePoints * Math.Pow(0.95d, i))
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
                    PerformancePointsMania = TotalPP;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
