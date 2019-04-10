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
using System.Linq;

namespace Shared.Helpers
{
    public class Logger
    {
        public static void Log(params object[] msg) => Debug(msg);

        public static void Debug(params object[] msg)
        {
            #if DEBUG
            const string prefix = "[DEBUG]";
            string m      = msg.Aggregate(string.Empty, (current, x) => current + (" " + x));

            Console.WriteLine(prefix + m, msg);
            #endif
        }

        public static void Info(params object[] msg)
        {
            const string prefix = "[INFO]";
            string m      = msg.Aggregate(string.Empty, (current, x) => current + (" " + x));

            Console.WriteLine(prefix + m, msg);
        }

        public static void Warn(params object[] msg)
        {
            const string prefix = "[WARN]";
            string m      = msg.Aggregate(string.Empty, (current, x) => current + (" " + x));

            Console.WriteLine(prefix + m, msg);
        }

        public static void Err(params object[] msg)
        {
            const string prefix = "[ERR]";
            string m      = msg.Aggregate(string.Empty, (current, x) => current + (" " + x));

            Console.WriteLine(prefix + m, msg);
        }

        public static void Fatal(params object[] msg)
        {
            const string prefix = "[Fatal]";
            string m      = msg.Aggregate(string.Empty, (current, x) => current + (" " + x));

            Console.WriteLine(prefix + m, msg);
            Environment.Exit(1);
        }
    }
}