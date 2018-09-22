using System.IO;
using Shared.Helpers;

namespace Sora.Helpers
{
    internal class LoginParser
    {
        public static Login ParseLogin(MStreamReader reader)
        {
            var l = new Login();
            using (var s = new StreamReader(reader.BaseStream))
            {
                l.Username = s.ReadLine();
                l.Password = s.ReadLine();
                var otherData = s.ReadLine()?.Split('|');
                if (otherData == null || otherData.Length < 5) return null;
                l.Build = otherData[0];
                l.Timezone = int.Parse(otherData[1]);
                l.DisplayLocation = otherData[2] == "1";
                l.SecurityHash = otherData[3];
                l.BlockNonFriendDMs = otherData[4] == "1";
            }
            return l;
        }
    }

    public class Login
    {
        public string Username;
        public string Password;
        public string Build;
        public int Timezone;
        public bool DisplayLocation;
        public string SecurityHash;
        public bool BlockNonFriendDMs;
    }
}
