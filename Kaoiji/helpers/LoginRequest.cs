using Kaoiji.consts;
using Kaoiji.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaoiji.helpers
{
    class LoginRequest
    {
        public static LoginData Parse(string b)
        {
            if (b == null)
                return null;

            LoginData data = new LoginData();

            string[] LoginInformation = b.Split('\n');
            if (LoginInformation.Length < 3)
                throw LoginExceptions.INVALID_LOGIN_DATA;

            data.Username = LoginInformation[0];
            data.Password = LoginInformation[1];

            string[] ClientInformation = LoginInformation[2].Split('|');
            data.OsuVersion = ClientInformation[0];
            int.TryParse(s: ClientInformation[1], out data.TimeOff);
            data.BlockNonFriendsDM = ClientInformation.Length >= 4 && ClientInformation[4] == "1";

            data.SecurityHash = new SecurityHash();
            string[] SHash = ClientInformation[3].Split(':');
            data.SecurityHash.DiskMD5 = SHash[4];
            data.SecurityHash.UniqueMD5 = SHash[5];
            return data;
        }
    }
}
