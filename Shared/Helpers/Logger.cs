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