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

using System;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Packets.Client;

namespace Sora.Packets.Server
{
    public class SpectatorFrames : IPacket
    {
        public readonly SpectatorFrame Frame;

        public SpectatorFrames(SpectatorFrame frames)
        {
            Frame = frames;
        }

        public PacketId Id => PacketId.ServerSpectateFrames;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Frame);
        }
    }
}