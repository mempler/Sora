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
using Sora.Helpers;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.Chat
{
    [EventClass]
    public class OnPublicMessageEvent
    {
        private readonly ChannelService _cs;

        public OnPublicMessageEvent(ChannelService cs) => _cs = cs;

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessage(BanchoSendIRCMessageArgs args)
        {
            Channel chan;
            switch (args.Message.ChannelTarget)
            {
                case "#spectator":
                    chan = args.pr.Spectator?.SpecChannel;
                    break;
                case "#multiplayer":
                    chan = args.pr.JoinedRoom?.Channel;
                    break;
                default:
                    chan = _cs.GetChannel(args.Message.ChannelTarget);
                    break;
            }

            if (chan == null)
            {
                args.pr += new ChannelRevoked(args.Message.ChannelTarget);
                return;
            }
            
            chan.WriteMessage(args.pr, args.Message.Message);
        }
    }
}
