using System.IO;
using System.Linq;

namespace Sora.Framework.Utilities
{
    public class LoginParser
    {
        public static Login ParseLogin(MStreamReader reader)
        {
            var l = new Login();
            using (var s = new StreamReader(reader.BaseStream))
            {
                
                l.Username = s.ReadLine();
                l.Password = s.ReadLine();
                var otherData = s.ReadLine()?.Split('|');
                if (otherData == null || otherData.Length < 5)
                    return null;
                l.Build = otherData[0];
                l.Timezone = sbyte.Parse(otherData[1]);
                l.DisplayLocation = otherData[2] == "1";
                l.SecurityHash = otherData[3];
                l.BlockNonFriendDMs = otherData[4] == "1";
            }

            return l;
        }
    }

    public class Login
    {
        public bool BlockNonFriendDMs;
        public string Build;
        public bool DisplayLocation;
        public string Password;
        public string SecurityHash;
        public sbyte Timezone;
        public string Username;

        public override int GetHashCode()
            => (BlockNonFriendDMs ? 1 : 0) +
               (DisplayLocation ? 1 : 0) +
               Password.GetHashCode() *
               SecurityHash.GetHashCode() *
               Username.GetHashCode() /
               Timezone;
    }
}
