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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JetBrains.Annotations;


#nullable enable

namespace Sora.Database.Models
{
    [Table("Friends")]
    [UsedImplicitly]
    public class DbFriend
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [Required]
        [ForeignKey(nameof(UserId))]
        public DbUser User { get; set; }

        [Required]
        public int FriendId { get; set; }
        
        [Required]
        [UsedImplicitly]
        [ForeignKey(nameof(FriendId))]
        public DbUser Friend { get; set; }

        public static IEnumerable<int> GetFriends(SoraDbContextFactory factory, int userId)
            => factory.Get()
                      .Friends
                      .Where(t => t.UserId == userId)
                      .Select(x => x.FriendId).ToList();

        public static async void AddFriend(SoraDbContextFactory factory, int userId, int friendId)
        {
            using var db = factory.GetForWrite();
            await db.Context.Friends.AddAsync(new DbFriend {UserId = userId, FriendId = friendId});
        }

        public static void RemoveFriend(SoraDbContextFactory factory, int userId, int friendId)
        {
            using var db = factory.GetForWrite();
            db.Context.RemoveRange(db.Context.Friends.Where(x => x.UserId == userId && x.FriendId == friendId));
        }
    }
}
