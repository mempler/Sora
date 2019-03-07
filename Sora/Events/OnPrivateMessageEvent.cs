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
using Shared.Models;
using Shared.Services;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnPrivateMessageEvent
    {
        private readonly Database _db;
        private readonly PresenceService _ps;

        public OnPrivateMessageEvent(Database db, PresenceService ps, ChannelService cs)
        {
            _db = db;
            _ps = ps;
        }

        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessage(BanchoSendIRCMessageArgs args)
        {
            Presence opr = _ps.GetPresence(Users.GetUserId(_db, args.Message.ChannelTarget));
            if (opr == null) return;
            Channel chan = opr.PrivateChannel;

            if (chan == null)
            {
                args.pr.Write(new ChannelRevoked(args.Message.ChannelTarget));
                return;
            }

            chan.WriteMessage(args.pr, args.Message.Message);
        }
    }
}