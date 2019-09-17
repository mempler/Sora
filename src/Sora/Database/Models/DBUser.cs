using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sora.Framework;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Database.Models
{
    public enum PasswordVersion
    {
        V0, // Md5 (never use this!)
        V1, // Md5 + BCrypt
        V2  // Md5 + SCrypt
    }
    
    [Table("Users")]
    public class DBUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public PasswordVersion PasswordVersion { get; set; }
        public string Permissions { get; set; }

        public static Task<DBUser> GetDBUser(SoraDbContextFactory factory, int userId)
            => factory.Get().Users.FirstOrDefaultAsync(u => u.Id == userId);

        public static Task<DBUser> GetDBUser(SoraDbContextFactory factory, string userName)
            => factory.Get().Users.FirstOrDefaultAsync(u => u.UserName == userName);
        
        public static Task<DBUser> GetDBUser(SoraDbContextFactory factory, User user)
            => GetDBUser(factory, user.Id);

        public User ToUser() => new User
        {
            Id = Id,
            UserName = UserName,
            Permissions = Permission.From(Permissions)
        };

        public bool IsPassword(string passMd5)
        {
            switch (PasswordVersion)
            {
                case PasswordVersion.V0:
                    return passMd5 == Password;
                case PasswordVersion.V1:
                    return Crypto.BCrypt.validate_password(passMd5, Password);
                case PasswordVersion.V2:
                    return Crypto.SCrypt.validate_password(passMd5, Password, PasswordSalt);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DBUser RegisterUser(
            SoraDbContextFactory factory,
            string userName, string password, bool md5 = true,
            PasswordVersion pwVersion = PasswordVersion.V1)
        {
            byte[] salt = null;
            string pw;
            switch (pwVersion)
            {
                case PasswordVersion.V0:
                    pw = !md5 ? Hex.ToHex(Crypto.GetMd5(password)) : password;
                    break;
                case PasswordVersion.V1:
                    pw = !md5 ? Hex.ToHex(Crypto.GetMd5(password)) : password;
                    pw = Crypto.BCrypt.generate_hash(pw);
                    break;
                case PasswordVersion.V2:
                    pw = !md5 ? Hex.ToHex(Crypto.GetMd5(password)) : password;
                    (pw, salt) = Crypto.SCrypt.generate_hash(pw);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pwVersion), pwVersion, "PasswordVersion MUST be either v0,v1 or v2!");
            }
            
            var user = new DBUser
            {
                UserName = userName,
                Password = pw,
                PasswordSalt = salt,
                PasswordVersion = pwVersion
            };

            using (var db = factory.GetForWrite())
            {
                db.Context.Add(user);
            }
            
            return user;
        }
    }
}