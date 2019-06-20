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

using System.Collections.Generic;
using Sora.Objects;

namespace Sora.Services
{
    public class PacketStreamService
    {
        private readonly Dictionary<string, PacketStream> _packetStreams = new Dictionary<string, PacketStream>();

        public PacketStreamService()
        {
            NewStream(new PacketStream("main"));
            NewStream(new PacketStream("admin"));
            NewStream(new PacketStream("lobby"));
        }

        public PacketStream GetStream(string name)
        {
            _packetStreams.TryGetValue(name, out var x);
            return x;
        }

        public void NewStream(PacketStream str)
        {
            _packetStreams[str.StreamName] = str;
        }

        public void RemoveStream(string name)
        {
            _packetStreams[name] = null;
        }
    }
}
