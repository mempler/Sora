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
using BCrypt;
using Shared.Enums;

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

        public bool IsPassword(string s) => BCryptHelper.CheckPassword(s, Password);

        public static int GetUserId(string username)
        {
            using (var db = new SoraContext())
            {
                var result = db.Users.Where(t => t.Username == username).Select(e => e).FirstOrDefault();
                return result?.Id ?? -1;
            }
        }

        public static Users GetUser(int userId)
        {
            if (userId == -1) return null;
            using (var db = new SoraContext())
            {
                var result = db.Users.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
                return result;
            }
        }

        public override string ToString() =>
            $"ID: {Id}, Email: {Email}, Privileges: {Privileges}";
    }
}
