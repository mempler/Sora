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

using Sora.Enums;

namespace Sora.Helpers
{
    public static class Accuracy
    {
        public static double GetAccuracy(
            int count300, int count100, int count50, int countMiss,
            int countGeki, int countKatu, PlayMode playMode)
        {
            var totalHits = count50 + count100 + count300 + countMiss;
            
            switch (playMode)
            {
                case PlayMode.Osu:
                    return totalHits > 0
                        ? (double) (count50 * 50 + count100 * 100 + count300 * 300) /
                          (totalHits * 300)
                        : 1;
                case PlayMode.Taiko:
                    return totalHits > 0
                        ? (double) (count100 * 150 + count300 * 300)
                          / (totalHits * 300)
                        : 1;
                case PlayMode.Ctb:
                    return totalHits > 0
                        ? (double) (count50 + count100 + count300)
                          / totalHits
                        : 1;
                case PlayMode.Mania:
                    totalHits += countGeki + countKatu;
                    return totalHits > 0
                        ? (double) (count50 * 50 + count100 * 100 + countKatu * 200 + (count300 + countGeki) * 300) /
                          (totalHits * 300)
                        : 1;
                default:
                    return 0;
            }
        }
    }
}
