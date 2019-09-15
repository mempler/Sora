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

using System;

namespace Sora.Framework.Enums
{
    // Source https://github.com/HoLLy-HaCKeR/HOPEless/blob/master/HOPEless/osu/Enums_Multiplayer.cs under MIT License.
    [Flags]
    public enum MultiSlotStatus : byte
    {
        Open = 1 << 0,
        Locked = 1 << 1,
        NotReady = 1 << 2,
        Ready = 1 << 3,
        NoMap = 1 << 4,
        Playing = 1 << 5,
        Complete = 1 << 6,
        Quit = 1 << 7,
        Joinable = Open | Quit,
        NotJoinable = Locked | NotReady | Ready | NoMap | Playing | Complete
    }
}
