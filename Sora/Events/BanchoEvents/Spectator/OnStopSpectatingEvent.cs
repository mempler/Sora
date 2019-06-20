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

using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;

namespace Sora.Events.BanchoEvents.Spectator
{
    [EventClass]
    public class OnStopSpectatingEvent
    {
        [Event(EventType.BanchoStopSpectating)]
        public void OnStopSpectating(BanchoStopSpectatingArgs args)
        {
            if (args.pr?.Spectator == null)
                return;
            var opr = args.pr.Spectator.BoundPresence;

            opr += new FellowSpectatorLeft(args.pr.User.Id);
            opr.Spectator.Broadcast(new SpectatorLeft(args.pr.User.Id));

            opr.Spectator.Left(args.pr);
            opr.Spectator.SpecChannel.LeaveChannel(args.pr);
            args.pr += new ChannelRevoked(opr.Spectator.SpecChannel);

            args.pr.Spectator = null;

            if (opr.Spectator.JoinedUsers > 0)
                return;

            opr.Spectator.Left(opr);
            opr.Spectator.SpecChannel.LeaveChannel(opr);
            opr += new ChannelRevoked(opr.Spectator.SpecChannel);
            opr.Spectator = null;
        }
    }
}
