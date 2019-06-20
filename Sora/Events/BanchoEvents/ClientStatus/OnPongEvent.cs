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

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnPongEvent
    {
        [Event(EventType.BanchoPong)]
        public void OnPong(BanchoPongArgs args)
        {
            if (args.pr.Spectator == null || args.pr.Spectator?.BoundPresence == args.pr) return;
            
            //args.pr.Spectator?.Broadcast(new UserPresence(args.pr));
            args.pr += new HandleUpdate(args.pr);
            args.pr.Spectator?.Broadcast(new HandleUpdate(args.pr));
        }
    }
}