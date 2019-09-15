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

using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchJoinEvent
    {
        private readonly EventManager _ev;

        public OnBanchoMatchJoinEvent(EventManager ev)
        {
            _ev = ev;
        }

        [Event(EventType.BanchoMatchJoin)]
        public async void OnBanchoMatchJoin(BanchoMatchJoinArgs args)
        {
            Lobby.Self.TryGet(args.matchId, out var room);
            if (room?.Join(args.pr, args.password.Replace(" ", "_")) == true)
                args.pr.Push(new MatchJoinSuccess(room));
            else
                args.pr.Push(new MatchJoinFail());

            room?.Update();

            await _ev.RunEvent(
                EventType.BanchoChannelJoin, new BanchoChannelJoinArgs {pr = args.pr, ChannelName = "#multiplayer"}
            );
        }
    }
}
