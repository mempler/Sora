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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sora.Helpers;

namespace Sora.Database.Models
{
    public class Users
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [NotMapped]
        public Permission Permissions { get; set; } = new Permission();

        [Required]
        [MaxLength]
        [DefaultValue("none")]
        public string AcquiredPermissions
        {
            get => Permissions?.ToString() ?? string.Empty;
            set => Permissions?.Add(value.Split(", "));
        }

        [Required]
        public ulong Achievements { get; set; }

        public bool IsPassword(string s) => BCrypt.validate_password(s, Password);

        public static int GetUserId(SoraDbContextFactory factory, string username)
        {
            return factory.Get().Users.FirstOrDefault(t => t.Username == username)?.Id ?? -1;
        }

        public static Users GetUser(SoraDbContextFactory factory, int userId)
        {
            return userId == -1 ? null : factory.Get().Users.FirstOrDefault(t => t.Id == userId);
        }

        public static Users GetUser(SoraDbContextFactory factory, string username)
        {
            return username == null ? null : factory.Get().Users.FirstOrDefault(t => t.Username == username);
        }

        public static void InsertUser(SoraDbContextFactory factory, params Users[] users)
        {
            using var db = factory.GetForWrite();

            db.Context.Users.AddRange(users);
        }

        public static Users NewUser(
            SoraDbContextFactory factory,
            string username, string password, string email = "", Permission permissions = null,
            bool insert = true
        )
        {
            var md5Pass = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            //string bcryptPass = BCrypt.Net.BCrypt.HashPassword(Encoding.UTF8.GetString(md5Pass));
            var bcryptPass = BCrypt.generate_hash(Hex.ToHex(md5Pass));
            Logger.Info(bcryptPass);

            var u = new Users
            {
                Username = username,
                Password = bcryptPass,
                Email = email,
                Permissions = permissions ?? new Permission()
            };

            if (insert)
                InsertUser(factory, u);

            return u;
        }

        public override string ToString() => $"ID: {Id}, Email: {Email}, Privileges: {Permissions}";

        public void ObtainAchievement(SoraDbContextFactory factory, Achievements ach)
        {
            using var db = factory.GetForWrite();

            Achievements |= ach.BitId;

            db.Context.Users.Update(this);
        }

        public bool AlreadyOptainedAchievement(Achievements ach) => (Achievements & ach.BitId) != 0;
    }
}
