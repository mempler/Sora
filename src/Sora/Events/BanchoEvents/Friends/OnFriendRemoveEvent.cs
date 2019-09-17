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

using System.Threading.Tasks;
using Sora.Attributes;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Utilities;

namespace Sora.Events.BanchoEvents.Friends
{
    [EventClass]
    public class OnFriendRemoveEvent
    {
        private readonly SoraDbContextFactory _factory;

        public OnFriendRemoveEvent(SoraDbContextFactory factory) => _factory = factory;

        [Event(EventType.BanchoFriendRemove)]
        public async Task OnFriendRemove(BanchoFriendRemoveArgs args)
        {
            var u = await DBUser.GetDBUser(_factory, args.FriendId);

            if (u != null)
                Logger.Info(
                    "%#F94848%" + args.pr.User.UserName,
                    "%#B342F4%(", args.pr.User.Id, "%#B342F4%)",
                    "%#FFFFFF%removed",
                    "%#F94848%" + u.UserName,
                    "%#B342F4%(", u.Id, "%#B342F4%)%#FFFFFF% as Friend!"
                );

            DBFriend.RemoveFriend(_factory, args.pr.User.Id, args.FriendId);
        }
    }
}
