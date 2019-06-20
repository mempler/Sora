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
    public class OnUserStatsRequestEvent
    {
        private readonly PresenceService _ps;

        public OnUserStatsRequestEvent(PresenceService ps) => _ps = ps;

        [Event(EventType.BanchoUserStatsRequest)]
        public void OnUserStatsRequest(BanchoUserStatsRequestArgs args)
        {
            foreach (var id in args.userIds)
            {
                if (id == args.pr.User.Id)
                    continue;

                var opr = _ps.GetPresence(id);
                if (opr == null)
                {
                    args.pr += new HandleUserQuit(new UserQuitStruct {UserId = id, ErrorState = ErrorStates.Ok});
                    continue;
                }

                args.pr += new HandleUpdate(opr);
            }
        }
    }
}
