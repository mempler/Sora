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
using Logger = Sora.Helpers.Logger;

namespace Sora.Events.BanchoEvents.Multiplayer
{
    [EventClass]
    public class OnBanchoMatchSettingsEvent
    {
        [Event(EventType.BanchoMatchChangeSettings)]
        public void OnBroadcastFrames(BanchoMatchChangeSettingsArgs args)
        {
            if (args.pr.JoinedRoom == null) return;
            if (args.pr.JoinedRoom.HostId != args.pr.User.Id) return;

            if (args.pr.JoinedRoom.Name != args.room.Name)
                Logger.Info("%#F94848%" + args.pr.User.Username,
                            "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                            "%#FFFFFF% renamed a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" +
                            args.pr.JoinedRoom.Name,
                            "%#B342F4%(", args.room.MatchId, "%#B342F4%)",
                            "%#FFFFFF%and is now called %#F94848%" +
                            args.room.Name,
                            "%#B342F4%(", args.room.MatchId, "%#B342F4%)"
                );
            
            Logger.Info("%#F94848%" + args.pr.User.Username,
                        "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                        "%#FFFFFF%changed the Settings of a %#f1fc5a%Multiplayer Room %#FFFFFF%called %#F94848%" + args.room.Name,
                        "%#B342F4%(", args.room.MatchId, "%#B342F4%)");

            args.pr.JoinedRoom.ChangeSettings(args.room);
        }
    }
}