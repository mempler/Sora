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

using Sora.Objects;
using RankedStatus = Sora.Enums.RankedStatus;

namespace Sora.Helpers
{
    public static class Fixer
    {
        public static RankedStatus FixRankedStatus(Cheesegull.RankedStatus rankedStatus)
        {
            switch (rankedStatus)
            {
                case Cheesegull.RankedStatus.Graveyard:
                    return RankedStatus.LatestPending;
                case Cheesegull.RankedStatus.Wip:
                    return RankedStatus.LatestPending;
                case Cheesegull.RankedStatus.Pending:
                    return RankedStatus.LatestPending;
                case Cheesegull.RankedStatus.Ranked:
                    return RankedStatus.Ranked;
                case Cheesegull.RankedStatus.Approved:
                    return RankedStatus.Approved;
                case Cheesegull.RankedStatus.Qualified:
                    return RankedStatus.Qualified;
                case Cheesegull.RankedStatus.Loved:
                    return RankedStatus.Loved;
                default:
                    return RankedStatus.Unknown;
            }
        }
    }
}