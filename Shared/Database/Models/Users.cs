using System.ComponentModel.DataAnnotations;
using System.Linq;
using BCrypt;
using Shared.Enums;

namespace Shared.Database.Models
{
    public class Users
    {
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
    }
}
