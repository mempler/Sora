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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sora.Enums;
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

        [Required]
        public Privileges Privileges { get; set; } = 0;

        [Required]
        public ulong Achievements { get; set; } = 0;

        public bool IsPassword(string s)
        {
            return BCrypt.validate_password(s, Password);
            //return BCrypt.Net.BCrypt.Verify(Encoding.UTF8.GetString(Hex.FromHex(s)), Password);
        }

        public static int GetUserId(SoraDbContextFactory factory, string username) =>
            factory.Get().Users.FirstOrDefault(t => t.Username == username)?.Id ?? -1;

        public static Users GetUser(SoraDbContextFactory factory, int userId) => 
            userId == -1 ? null : factory.Get().Users.FirstOrDefault(t => t.Id == userId);

        public static Users GetUser(SoraDbContextFactory factory, string username) =>
            username == null ? null : factory.Get().Users.FirstOrDefault(t => t.Username == username);

        public static void InsertUser(SoraDbContextFactory factory, params Users[] users)
        {
            using DatabaseWriteUsage db = factory.GetForWrite();
            
            db.Context.Users.AddRange(users);
        }
        
        public static Users NewUser(
            SoraDbContextFactory factory,
            
            string username, string password, string email = "", Privileges privileges = 0,
            
            bool insert = true
            )
        {
            byte[] md5Pass = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            //string bcryptPass = BCrypt.Net.BCrypt.HashPassword(Encoding.UTF8.GetString(md5Pass));
            string bcryptPass = BCrypt.generate_hash(Hex.ToHex(md5Pass));
            Logger.Info(bcryptPass);
            
            Users u = new Users
            {
                Username   = username,
                Password   = bcryptPass,
                Email      = email,
                Privileges = privileges
            };
            
            if (insert)
                InsertUser(factory, u);

            return u;
        }

        public override string ToString()
            => $"ID: {Id}, Email: {Email}, Privileges: {Privileges}";

        public bool HasPrivileges(Privileges privileges) => (Privileges & privileges) != 0;

        public void ObtainAchievement(SoraDbContextFactory factory, Achievements ach)
        {
            using DatabaseWriteUsage db = factory.GetForWrite();
            
            Achievements |= ach.BitId;
            
            db.Context.Users.Update(this);
        }

        public bool AlreadyOptainedAchievement(Achievements ach) => (Achievements & ach.BitId) != 0;
    }
}