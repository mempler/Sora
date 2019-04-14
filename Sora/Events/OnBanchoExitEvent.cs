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
using Shared.Helpers;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoExitEvent
    {
        private readonly PresenceService _pcs;
        private readonly PacketStreamService _pss;

        public OnBanchoExitEvent(PresenceService pcs, PacketStreamService pss)
        {
            _pcs = pcs;
            _pss = pss;
        }
        
        [Event(EventType.BanchoExit)]
        public void OnBanchoExit(BanchoExitArgs args)
        {
            PacketStream mainStream = _pss.GetStream("main");

            Logger.Info("%#F94848%" + args.pr.User.Username,
                        "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                        "%#FFFFFF%has Disconnected!");
            
            mainStream?.Broadcast(new HandleUserQuit(new UserQuitStruct
            {
                UserId     = args.pr.User.Id,
                ErrorState = args.err
            }), args.pr);
        }
    }
}