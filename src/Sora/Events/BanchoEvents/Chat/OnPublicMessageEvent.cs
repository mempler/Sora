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
using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
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
            Channel channel;
            
            switch (args.Message.ChannelTarget)
            {
                case "#spectator":
                    channel = args.pr.Spectator?.Channel;
                    break;
                case "#multiplayer":
                    channel = args.pr.ActiveMatch?.Channel;
                    break;
                default:
                    _cs.TryGet(args.Message.ChannelTarget, out channel);
                    break;
            }

            if (channel == null)
            {
                args.pr.Push(new ChannelRevoked(args.Message.ChannelTarget));
                return;
            }

            args.Message.Username = args.pr.User.UserName;

            channel.Push(new SendIrcMessage(args.Message), args.pr);
        }
    }
}
