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
            ulong count300, ulong count100, ulong count50, ulong countMiss,
            ulong countGeki, ulong countKatu, PlayMode playMode)
        {
            if (count300 + count100 + count100 + count50 + countMiss + countGeki + countKatu == 0)
                return 0;
            
            switch (playMode)
            {
                case PlayMode.Osu:
                    return (count50 * 50 + count100 * 100 + count300 * 300) / ((countMiss + count50 + count300 +
                                                                                count100) * (double) 300);
                case PlayMode.Taiko:
                    return (count50 * 50 + count300 * 100) / ((double) (countMiss + count100 + count300) * 100);
                case PlayMode.Ctb:
                    return (count300 + count100 + count50) / (double) count300 + count100 + count50 + count300 +
                           countKatu;
                case PlayMode.Mania:
                    return (count50 * 50 + count100 * 100 + countKatu * 200 + count300 * 300 + countGeki * 300) /
                           ((double) (countMiss + count50 + count100 + count300 + countGeki + countKatu) * 300);
                default:
                    return 0;
            }
        }
    }
}