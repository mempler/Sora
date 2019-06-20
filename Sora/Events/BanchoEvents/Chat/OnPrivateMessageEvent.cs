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
using Sora.Database;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;
using Logger = Sora.Helpers.Logger;
using Users = Sora.Database.Models.Users;

namespace Sora.Events.BanchoEvents.Chat
{
    [EventClass]
    public class OnPrivateMessageEvent
    {
        private readonly SoraDbContextFactory _factory;
        private readonly PresenceService _ps;

        public OnPrivateMessageEvent(SoraDbContextFactory factory, PresenceService ps, ChannelService cs)
        {
            _factory = factory;
            _ps = ps;
        }

        [Event(EventType.BanchoSendIrcMessagePrivate)]
        public void OnPrivateMessage(BanchoSendIRCMessageArgs args)
        {
            Presence opr = _ps.GetPresence(Users.GetUserId(_factory, args.Message.ChannelTarget));
            if (opr == null) return;
            Channel chan = opr.PrivateChannel;
   
            if (chan == null)
            {
                args.pr += new ChannelRevoked(args.Message.ChannelTarget);
                return;
            }

            Logger.Info("%#F94848%" + args.pr.User.Username,
                        "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                        "%#f1fc5a%(Private Message)",
                        "%#FFFFFF%=>",
                        "%#F94848%" + opr.User.Username,
                        "%#B342F4%(", opr.User.Id, "%#B342F4%)");

            chan.WriteMessage(args.pr, args.Message.Message);
        }
    }
}