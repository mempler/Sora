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
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.Chat
{
    [EventClass]
    public class OnChannelJoinEvent
    {
        private readonly ChannelService _cs;

        public OnChannelJoinEvent(ChannelService cs) => _cs = cs;

        [Event(EventType.BanchoChannelJoin)]
        public void OnChannelJoin(BanchoChannelJoinArgs args)
        {
            Channel channel;
            switch (args.ChannelName)
            {
                case "#spectator":
                    channel = args.pr.Spectator?.SpecChannel;
                    break;
                case "#multiplayer":
                    channel = args.pr.JoinedRoom?.Channel;
                    break;
                default:
                    channel = _cs.GetChannel(args.ChannelName);
                    break;
            }

            if (channel == null)
            {
                args.pr += new ChannelRevoked(args.ChannelName);
                return;
            }

            channel.LeaveChannel(args.pr); // leave channel before joining to fix some Issues.

            if (channel.JoinChannel(args.pr))
                args.pr += new ChannelJoinSuccess(channel);

            channel.BoundStream?.Broadcast(new ChannelAvailable(channel));
        }
    }
}
