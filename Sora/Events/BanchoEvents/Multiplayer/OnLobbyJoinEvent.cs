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
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.Multiplayer
{
    [EventClass]
    public class OnLobbyJoinEvent
    {
        private readonly PacketStreamService _ps;
        private readonly MultiplayerService _ms;

        public OnLobbyJoinEvent(PacketStreamService ps, MultiplayerService ms)
        {
            _ps = ps;
            _ms = ms;
        }
        
        [Event(EventType.BanchoLobbyJoin)]
        public void OnLobbyJoin(BanchoLobbyJoinArgs args)
        {
            PacketStream                 lobbyStream = _ps.GetStream("lobby");
            IEnumerable<MultiplayerRoom> rooms       = _ms.GetRooms();
            foreach (MultiplayerRoom room in rooms)
                args.pr.Write(new MatchNew(room));

            lobbyStream.Join(args.pr);
        }
    }
}