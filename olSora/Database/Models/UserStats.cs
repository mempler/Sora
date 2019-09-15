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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sora.Enums;

namespace Sora.Database.Models
{
    public class UserStats
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }

        [Required]
        [DefaultValue(0)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public CountryIds CountryId { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public static UserStats GetUserStats(SoraDbContextFactory factory, int userId)
        {
            using var db = factory.GetForWrite();

            var val = db.Context.UserStats.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
            if (val != null)
                return val;

            db.Context.UserStats.Add(val = new UserStats {Id = userId, CountryId = 0});
            return val;
        }

        // ReSharper disable once UnusedMember.Global
        public static UserStats GetUserStats(SoraDbContextFactory factory, Users user)
            => GetUserStats(factory, user.Id);
    }
}
