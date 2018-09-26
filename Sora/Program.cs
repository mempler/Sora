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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using NLog;
using Shared.Database;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;
using Sora.Server;

namespace Sora
{

    internal static class Program
    {
        public static Logger Logger;
        private static HttpServer _server;

        #if _WINDOWS // This only works for windows so we have to remove it if it's linux or any other OS!
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);
        public delegate bool HandlerRoutine(CtrlTypes ctrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endif

        private static void Initialize() {
            Logger = LogManager.GetCurrentClassLogger();
            try
            {
                Logger.Info("Start Initalization");
                SetConsoleCtrlHandler(ConsoleCtrlCheck, true);
                
                _server = new HttpServer(Config.ReadConfig().Server.Port);
                Logger.Info("Initalization Success");
                using (var db = new SoraContext()) { }

                AppDomain.CurrentDomain.UnhandledException += delegate(object ex, UnhandledExceptionEventArgs e) { Logger.Error(ex); };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Handlers.InitHandlers(Assembly.GetEntryAssembly(), false);
            Handlers.ExecuteHandler(HandlerTypes.Initializer);
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Initialize();
            LoadPlugins();
            _server.Run();
        }

        private static void LoadPlugins()
        {
            Logger.Info("Start loading plugins!");
            if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");

            foreach (var f in Directory.GetFiles("Plugins")) // Press F for File
            {
                var file = Assembly.LoadFrom(f);
                var fs = file.GetManifestResourceStream(file.GetName().Name +".plugin.xml");
                if (fs == null) continue;
                var doc = XDocument.Load(fs);
                if (doc.Root != null)
                    Logger.Info(
                        $"Loaded plugin: {doc.Root.Attribute("Name")?.Value}. Version: {doc.Root.Attribute("Version")?.Value}");
                Handlers.InitHandlers(file, false);
            }
            Logger.Info("Finish loading plugins!");
        }

        #if _WINDOWS
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            _server.Stop();
            Thread.Sleep(500);
            return true;
        }
        #endif
    }
}
