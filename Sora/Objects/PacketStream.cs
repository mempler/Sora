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
using System.Linq;
using Sora.Allocation;
using Sora.Helpers;
using Sora.Interfaces;

namespace Sora.Objects
{
    public class PacketStream : DynamicValues
    {
        private readonly Dictionary<string, Presence> _joinedPresences = new Dictionary<string, Presence>();

        public PacketStream(string name) => StreamName = name;

        public string StreamName { get; }

        public int JoinedUsers => _joinedPresences.Count;

        public void Join(Presence pr)
        {
            if (!_joinedPresences.ContainsKey(pr.Token))
                _joinedPresences.Add(pr.Token, pr);
        }

        public void Left(Presence pr)
        {
            if (_joinedPresences.ContainsKey(pr.Token))
                _joinedPresences.Remove(pr.Token);
        }

        public void Left(string token)
        {
            _joinedPresences.Remove(token);
        }

        public void Broadcast(IPacket packet, params Presence[] ignorePresences)
        {
            foreach ((string key, Presence value) in _joinedPresences)
            {
                if (value == null)
                {
                    Left(key);
                    continue;
                }

                if (ignorePresences.Contains(value))
                    continue;

                if (packet == null)
                    Logger.Err("PACKET IS NULL!");

                value.Write(packet);
            }
        }
    }
}
