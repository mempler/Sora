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

namespace Sora.Events.BanchoEvents.Spectator
{
    [EventClass]
    public class OnStartSpectatingEvent
    {
        private readonly PresenceService _ps;

        public OnStartSpectatingEvent(PresenceService ps)
        {
            _ps = ps;
        }
        
        [Event(EventType.BanchoStartSpectating)]
        public void OnStartSpectating(BanchoStartSpectatingArgs args)
        {
            Presence opr = _ps.GetPresence(args.SpectatorHostID);
            if (opr == null) return;
            if (opr.Spectator == null)
            {
                opr.Spectator = new SpectatorStream($"spec-{args.SpectatorHostID}", opr);
                opr.Spectator.Join(opr);
                opr.Write(new ChannelJoinSuccess(opr.Spectator.SpecChannel));
            }

            args.pr.Spectator = opr.Spectator;

            opr.Spectator.Join(args.pr);
            if (opr.Spectator.SpecChannel.JoinChannel(args.pr))
                args.pr.Write(new ChannelJoinSuccess(opr.Spectator.SpecChannel));

            opr.Spectator.Broadcast(new FellowSpectatorJoined(args.pr.User.Id));
            opr.Write(new SpectatorJoined(args.pr.User.Id));
        }
    }
}