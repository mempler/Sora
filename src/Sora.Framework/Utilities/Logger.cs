using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace Sora.Framework.Utilities
{
    public static class LCol
    {
        public const string WHITE = "%#FFFFFF%";
        public const string BLACK = "%#000000%";
        public const string YELLOW = "%#F1FC5A%";
        public const string PURPLE = "%#B342F4%";
        public const string RED = "%#F94848%";
        public const string DARK_RED = "%#800000%";
        public const string BLUE = "%#1E88E5%";
    }

    public static class Logger
    {
        public static void Log(params object[] msg)
        {
            Debug(msg);
        }

        private static void PrintColorized(string Prefix, Color PrefixColor, bool exit, IEnumerable<object> msg)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var color = PrefixColor;

            var m = msg.Aggregate(string.Empty, (current, x) => current + " " + x);
            Console.WriteFormatted(Prefix, color);

            color = Color.White;
            foreach (var x in Regex.Split(m, @"\%(.*?)\%"))
            {
                if (!x.StartsWith("#") || !Hex.IsHex(x.TrimStart('#')) || x.Length != 7)
                {
                    Console.WriteFormatted(x, color);
                    continue;
                }

                color = Color.FromArgb(int.Parse(x.Replace("#", "").ToUpper().Trim(), NumberStyles.HexNumber));
            }

            Console.WriteLine();

            if (exit)
                Environment.Exit(1);
        }

        public static void Debug(params object[] msg)
            #if DEBUG
            => PrintColorized("[DEBUG]", Color.FromArgb(0xCECECE), false, msg);
        #else
            { }
        #endif

        public static void Info(params object[] msg)
        {
            PrintColorized("[INFO]", Color.FromArgb(0x37B6FF), false, msg);
        }

        public static void Warn(params object[] msg)
        {
            PrintColorized("[WARNING]", Color.FromArgb(0xFFDA38), false, msg);
        }

        public static void Err(params object[] msg)
        {
            PrintColorized("[ERROR]", Color.FromArgb(0xFF4C4C), false, msg);
        }

        public static void Fatal(params object[] msg)
        {
            PrintColorized("[FATAL]", Color.FromArgb(0x910000), true, msg);
        }
    }
}
