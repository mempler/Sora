using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using NLog;
using Sora.Handler;
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
                _server = new HttpServer(5001);
                Logger.Info("Initalization Success");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Initialize();
            LoadPlugins();
            Handlers.InitHandlers(Assembly.GetAssembly(typeof(Program)), false);
            _server.Run();
            while(true) { Thread.Sleep(500); } // Prevent tons of CPU Usage
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
