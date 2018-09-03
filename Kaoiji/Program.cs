using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Newtonsoft.Json;

using Kaoiji.server;
using Kaoiji.handler;

namespace Kaoiji
{
    class Program
    {
        private static string PluginDirectory = Path.Combine(Environment.CurrentDirectory, "plugins");

        /// <summary>
        /// Protocol Version. Current osu! version uses 19
        /// </summary>
        public const int ProtocolVersion = 19;
        /// <summary>
        /// List of Loaded plugins.
        /// </summary>
        public static List<Plugin> Plugins = new List<Plugin>();

        static void Main(string[] args)
        {
            // On Exit we want to unload all plugins.
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            // Load all Plugins.
            LoadPlugins();
            // Initialize handlers AFTER loading Plugins.
            Handlers.Init();
            // Run HTTP Server
            HttpServer serv = new HttpServer(80);
            serv.Run();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            UnloadPlugins();
        }

        /// <summary>
        /// LoadPlugins is supposed to load all Plugins inside "plugins" Directory.
        /// </summary>
        static void LoadPlugins()
        {
            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);

            foreach (string pluginDir in Directory.GetDirectories(PluginDirectory))
            {
                try
                {
                    string pMetaRaw = Path.Combine(pluginDir, "Plugin.json");
                    if (!File.Exists(pMetaRaw))
                        continue;

                    PluginMeta pMeta = JsonConvert.DeserializeObject<PluginMeta>(File.ReadAllText(pMetaRaw));
                    Assembly assembly = Assembly.LoadFrom(Path.Combine(pluginDir, pMeta.Plugin));

                    // Thanks to HoLLy ^^,
                    foreach (Type type in assembly.GetTypes().Where(a => a.IsSubclassOf(typeof(Plugin))))
                    {
                        Plugin p = (Plugin)Activator.CreateInstance(type);
                        p.pMeta = pMeta;
                        Plugins.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            foreach (Plugin p in Plugins)
            {
                try
                {
                    Console.WriteLine($"Try to Enable \"{p.pMeta.Name}\"");
                    p.OnStart();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    p.OnStop();
                }
            }
        }
        /// <summary>
        /// UnloadPlugins is supposed to unload all plugins.
        /// </summary>
        static void UnloadPlugins()
        {
            for (int i = 0; i < Plugins.Count; i++)
            {
                Plugin p = Plugins[i];
                try
                {
                    Console.WriteLine($"Try to Disable \"{p.pMeta.Name}\"");
                    p.OnStop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Plugins.Remove(p);
                i--;
            }
        }
    }
}
