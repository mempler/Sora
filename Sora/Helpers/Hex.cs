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

using System;
using System.Text;

namespace Sora.Helpers
{
    public static class Hex
    {
        // https://stackoverflow.com/questions/724862/converting-from-hex-to-string
        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw                                  = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++) raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return raw;
        }

        public static string ToHex(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte d in data)
                hex.AppendFormat("{0:x2}", d);
            return hex.ToString();
        }
    }
}