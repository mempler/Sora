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
                l.Timezone = byte.Parse(otherData[1]);
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
        public byte Timezone;
        public bool DisplayLocation;
        public string SecurityHash;
        public bool BlockNonFriendDMs;
    }
}
