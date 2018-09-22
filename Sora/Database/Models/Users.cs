using System.ComponentModel.DataAnnotations;
using BCrypt;
using Sora.Enums;

namespace Sora.Database.Models
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
    }
}
