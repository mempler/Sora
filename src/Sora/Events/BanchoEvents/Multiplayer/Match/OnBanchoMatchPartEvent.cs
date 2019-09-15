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
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchPartEvent
    {
        private readonly EventManager _ev;

        public OnBanchoMatchPartEvent(EventManager ev) => _ev = ev;

        [Event(EventType.BanchoMatchPart)]
        public async void OnBanchoMatchPart(BanchoMatchPartArgs args)
        {
            if (args.pr.ActiveMatch == null)
                return;

            var room = args.pr.ActiveMatch;
            room.Leave(args.pr);
            if (room.HostId == args.pr.User.Id)
                room.SetRandomHost();

            await _ev.RunEvent(
                EventType.BanchoChannelLeave, new BanchoChannelLeaveArgs {pr = args.pr, ChannelName = "#multiplayer"}
            );

            if (room.HostId != -1)
            {
                room.Update();
                return;
            }

            Logger.Info(
                "Detected Empty %#f1fc5a%Multiplayer Room %#FFFFFF%called",
                "%#F94848%" + room.Name,
                "%#B342F4%(", room.MatchId, "%#B342F4%)%#FFFFFF% Cleaning up..."
            );

            room.Disband();
        }
    }
}
