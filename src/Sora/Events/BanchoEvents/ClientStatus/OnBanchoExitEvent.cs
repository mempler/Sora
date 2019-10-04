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
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnBanchoExitEvent
    {
        private readonly PresenceService _ps;
        
        public OnBanchoExitEvent(PresenceService ps)
        {
            _ps = ps;
        }

        [Event(EventType.BanchoExit)]
        public void OnBanchoExit(BanchoExitArgs args)
        {
            Logger.Info(
                "%#F94848%" + args.pr.User.UserName,
                "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                "%#FFFFFF%has Disconnected!"
            );

            _ps.Push(new HandleUserQuit(new UserQuitStruct {UserId = args.pr.User.Id, ErrorState = args.err}));
            
            _ps.Pop(args.pr.Token);
        }
    }
}
