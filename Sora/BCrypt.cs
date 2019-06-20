using System.Runtime.InteropServices;

namespace Sora
{
    public class BCrypt
    {
        [DllImport(@"lib/BCrypt")]
        public static extern string generate_hash(string password, int workload = 12);

        [DllImport(@"lib/BCrypt")]
        public static extern bool validate_password(string password, string hash);
    }
}
