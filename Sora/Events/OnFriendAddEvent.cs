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
using Shared.Models;
using Shared.Services;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnFriendAddEvent
    {
        private readonly Database _db;

        public OnFriendAddEvent(Database db)
        {
            _db = db;
        }
        
        public void OnFriendAdd(BanchoFriendAddArgs args)
        {
            Friends.AddFriend(_db, args.pr.User.Id, args.FriendId);
        }
    }
}