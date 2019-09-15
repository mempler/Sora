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
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchCreateEvent
    {
        private readonly EventManager _ev;

        public OnBanchoMatchCreateEvent(EventManager ev)
        {
            _ev = ev;
        }

        [Event(EventType.BanchoMatchCreate)]
        public async void OnBanchoMatchCreate(BanchoMatchCreateArgs args)
        {
            args.room.Password = args.room.Password.Replace(" ", "_");
            Lobby.Self.Push(args.room);

            Logger.Info(
                "%#F94848%" + args.pr.User.UserName,
                "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                "%#FFFFFF%has created a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" + args.room.Name,
                "%#B342F4%(", args.room.MatchId, "%#B342F4%)"
            );

            if (args.room.Join(args.pr, args.room.Password))
                args.pr.Push(new MatchJoinSuccess(args.room));
            else
                args.pr.Push(new MatchJoinFail());

            args.room.Update();

            await _ev.RunEvent(
                EventType.BanchoChannelJoin, new BanchoChannelJoinArgs {pr = args.pr, ChannelName = "#multiplayer"}
            );
        }
    }
}
