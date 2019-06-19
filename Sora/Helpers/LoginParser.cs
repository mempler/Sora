#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using System.IO;

namespace Sora.Helpers
{
    internal class LoginParser
    {
        public static Login ParseLogin(MStreamReader reader)
        {
            Login l = new Login();
            using (StreamReader s = new StreamReader(reader.BaseStream))
            {
                l.Username = s.ReadLine();
                l.Password = s.ReadLine();
                string[] otherData = s.ReadLine()?.Split('|');
                if (otherData == null || otherData.Length < 5) return null;
                l.Build             = otherData[0];
                l.Timezone          = byte.Parse(otherData[1]);
                l.DisplayLocation   = otherData[2] == "1";
                l.SecurityHash      = otherData[3];
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
        public byte Timezone;
        public string Username;

        public override int GetHashCode()
        {
            return (BlockNonFriendDMs ? 1 : 0) +
                   (DisplayLocation ? 1 : 0) +
                   Password.GetHashCode() *
                   SecurityHash.GetHashCode() *
                   Username.GetHashCode() /
                   Timezone;
        }
    }
}