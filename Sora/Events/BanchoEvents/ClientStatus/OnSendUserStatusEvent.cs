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
using Sora.Services;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnSendUserStatusEvent
    {
        private readonly PacketStreamService _pss;

        public OnSendUserStatusEvent(PacketStreamService pss) => _pss = pss;

        [Event(EventType.BanchoSendUserStatus)]
        public void OnSendUserStatus(BanchoSendUserStatusArgs args)
        {
            args.pr["STATUS"] = args.status;
            args.pr["IS_RELAXING"] = (args.status.CurrentMods & Mod.Relax) != 0;
            //Logger.Debug("OnSendUserStatusEvent", "Is Relaxing", args.pr.Relax);

            var main = _pss.GetStream("main");
            //main.Broadcast(new UserPresence(args.pr));
            main.Broadcast(new HandleUpdate(args.pr));
        }
    }
}
