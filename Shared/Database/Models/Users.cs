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

namespace Shared.Database.Models
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

        public bool IsPassword(string s) => BCrypt.Net.BCrypt.Verify(Encoding.UTF8.GetString(Hex.FromHex(s)), this.Password);

        public static int GetUserId(string username)
        {
            using (SoraContext db = new SoraContext())
            {
                Users result = db.Users.Where(t => t.Username == username).Select(e => e).FirstOrDefault();
                return result?.Id ?? -1;
            }
        }

        public static Users GetUser(int userId)
        {
            if (userId == -1) return null;
            using (SoraContext db = new SoraContext())
            {
                Users result = db.Users.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
                return result;
            }
        }

        public static void InsertUser(params Users[] users)
        {
            using (SoraContext db = new SoraContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }
        }

        public static Users NewUser(string username, string password, string email = "", Privileges privileges = 0, bool insert = true)
        {
            var md5Pass = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            
            string bcryptPass = BCrypt.Net.BCrypt.HashPassword(Encoding.UTF8.GetString(md5Pass));
            Users u = new Users
            {
                Username = username,
                Password = bcryptPass,
                Email = email,
                Privileges = privileges
            };
            if (insert)
                InsertUser(u);

            return u;
        }

        public override string ToString() =>
            $"ID: {this.Id}, Email: {this.Email}, Privileges: {this.Privileges}";

        public bool HasPrivileges(Privileges privileges) => (this.Privileges & privileges) != 0;
    }
}
