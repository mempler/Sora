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

using Sora.Framework.Enums;

namespace Sora.Framework.Utilities
{
    public static class Fixer
    {
        public static RankedStatus FixRankedStatus(Pisstaube.RankedStatus rankedStatus)
        {
            switch (rankedStatus)
            {
                case Pisstaube.RankedStatus.Graveyard:
                    return RankedStatus.LatestPending;
                case Pisstaube.RankedStatus.Wip:
                    return RankedStatus.LatestPending;
                case Pisstaube.RankedStatus.Pending:
                    return RankedStatus.LatestPending;
                case Pisstaube.RankedStatus.Ranked:
                    return RankedStatus.Ranked;
                case Pisstaube.RankedStatus.Approved:
                    return RankedStatus.Approved;
                case Pisstaube.RankedStatus.Qualified:
                    return RankedStatus.Qualified;
                case Pisstaube.RankedStatus.Loved:
                    return RankedStatus.Loved;
                default:
                    return RankedStatus.Unknown;
            }
        }
    }
}
