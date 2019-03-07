#region copyright

/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shared.Enums;
using Shared.Helpers;
using Shared.Services;

namespace Shared.Models
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

        public bool IsPassword(string s) => BCrypt.Net.BCrypt.Verify(Encoding.UTF8.GetString(Hex.FromHex(s)), Password);

        public static int GetUserId(Database db, string username) =>
            db.Users.FirstOrDefault(t => t.Username == username)?.Id ?? -1;

        public static Users GetUser(Database db, int userId) => 
            userId == -1 ? null : db.Users.FirstOrDefault(t => t.Id == userId);

        public static void InsertUser(Database db, params Users[] users)
        {
            db.Users.AddRange(users);
            db.SaveChanges();
        }

        public static Users NewUser(
            Database db,
            
            string username, string password, string email = "", Privileges privileges = 0,
            bool insert = true)
        {
            byte[] md5Pass = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));

            string bcryptPass = BCrypt.Net.BCrypt.HashPassword(Encoding.UTF8.GetString(md5Pass));
            Users u = new Users
            {
                Username   = username,
                Password   = bcryptPass,
                Email      = email,
                Privileges = privileges
            };
            if (insert)
                InsertUser(db, u);

            return u;
        }

        public override string ToString() => $"ID: {Id}, Email: {Email}, Privileges: {Privileges}";

        public bool HasPrivileges(Privileges privileges) => (Privileges & privileges) != 0;
    }
}