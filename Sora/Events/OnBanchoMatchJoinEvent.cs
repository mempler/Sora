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

using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchJoinEvent
    {
        private readonly MultiplayerService _ms;
        private readonly EventManager.EventManager _ev;

        public OnBanchoMatchJoinEvent(MultiplayerService ms, EventManager.EventManager ev)
        {
            _ms = ms;
            _ev = ev;
        }
    
        [Event(EventType.BanchoMatchJoin)]
        public void OnBanchoMatchJoin(BanchoMatchJoinArgs args)
        {
            MultiplayerRoom room = _ms.GetRoom(args.matchId);
            if (room != null && room.Join(args.pr, args.password.Replace(" ", "_")))
                args.pr.Write(new MatchJoinSuccess(room));
            else
                args.pr.Write(new MatchJoinFail());

            room?.Update();

            _ev.RunEvent(EventType.BanchoChannelJoin, new BanchoChannelJoinArgs
            {
                pr          = args.pr,
                ChannelName = "#multiplayer"
            });
        }
    }
}