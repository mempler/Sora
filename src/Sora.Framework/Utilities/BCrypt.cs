using System.Runtime.InteropServices;

namespace Sora.Framework.Utilities
{
    public class BCrypt
    {
        /// <summary>
        /// Generate BCrypt Hashed Password. this is really cpu heavy which means this should be run only once per user!
        /// </summary>
        /// <param name="password">Password Md5 String which is required to verify the Password</param>
        /// <param name="rounds">How much the Hash should be rounded.</param>
        /// <returns></returns>
        [DllImport(@"lib/BCrypt")]
        public static extern string generate_hash(string password, int rounds = 12);

        /// <summary>
        /// Validate if a password is equal to Password BCrypt Hash
        /// </summary>
        /// <param name="password">Password Md5</param>
        /// <param name="hash">Password BCrypt Hash</param>
        /// <returns></returns>
        [DllImport(@"lib/BCrypt")]
        public static extern bool validate_password(string password, string hash);
    }
}
