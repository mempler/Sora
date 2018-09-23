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
            _server.Run().Join(); // > owo - Justin
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
