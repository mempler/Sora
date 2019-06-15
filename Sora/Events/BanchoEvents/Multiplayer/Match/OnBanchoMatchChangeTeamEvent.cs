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

using System;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events.BanchoEvents.Multiplayer
{
    [EventClass]
    public class OnBanchoMatchChangeTeamEvent
    {
        [Event(EventType.BanchoMatchChangeTeam)]
        public void OnBanchoMatchChangeTeam(BanchoMatchChangeTeamArgs args)
        {
            MultiplayerSlot slot = args.pr.JoinedRoom?.GetSlotByUserId(args.pr.User.Id);
            if (slot == null) return;

            switch (slot.Team)
            {
                case MultiSlotTeam.Blue:
                    slot.Team = MultiSlotTeam.Red;
                    break;
                case MultiSlotTeam.Red:
                    slot.Team = MultiSlotTeam.Blue;
                    break;
                case MultiSlotTeam.NoTeam:
                    slot.Team = new Random().Next(1) == 1 ? MultiSlotTeam.Red : MultiSlotTeam.Blue;
                    break;
                default:
                    slot.Team = MultiSlotTeam.NoTeam;
                    break;
            }

            args.pr.JoinedRoom.Update();
        }
    }
}