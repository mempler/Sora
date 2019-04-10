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
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchChangeSlotEvent
    {   
        [Event(EventType.BanchoMatchChangeSlot)]
        public void OnBanchoMatchChangeSlot(BanchoMatchChangeSlotArgs args)
        {
            if (args.pr.JoinedRoom == null) return;
            if (args.SlotId > 16) return;
            MultiplayerSlot newSlot = args.pr.JoinedRoom.Slots[args.SlotId];
            if (newSlot.UserId != -1) return;

            MultiplayerSlot oldSlot = args.pr.JoinedRoom.GetSlotByUserId(args.pr.User.Id);

            args.pr.JoinedRoom.SetSlot(newSlot, oldSlot);
            args.pr.JoinedRoom.ClearSlot(oldSlot);
        }
    }
}