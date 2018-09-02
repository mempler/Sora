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
        public const int ProtocolVersion = 19;
        private static string PluginDirectory = Path.Combine(Environment.CurrentDirectory, "plugins");
        public static List<Plugin> Plugins = new List<Plugin>();
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            LoadPlugins();
            Handlers.init();
            HttpServer serv = new HttpServer(80);
            serv.Run();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            UnloadPlugins();
        }


        static void LoadPlugins()
        {
            if (!Directory.Exists(PluginDirectory))
                Directory.CreateDirectory(PluginDirectory);
            foreach (string pluginDir in Directory.GetDirectories(PluginDirectory))
            {
                try
                {
                    if (!File.Exists(Path.Combine(pluginDir, "Plugin.json")))
                        continue;
                    string pMetaRaw = Path.Combine(pluginDir, "Plugin.json");
                    PluginMeta pMeta = JsonConvert.DeserializeObject<PluginMeta>(File.ReadAllText(pMetaRaw));
                    Assembly assembly = Assembly.LoadFrom(Path.Combine(pluginDir, pMeta.Plugin));

                    // Thanks to HoLLy ^^,
                    foreach (Type type in assembly.GetTypes().Where(a => a.IsSubclassOf(typeof(Plugin))))
                    {
                        Plugin p = (Plugin)Activator.CreateInstance(type);
                        p.pMeta = pMeta;
                        Plugins.Add(p);
                    }
                        
                } catch (Exception ex)
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
                } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        static void UnloadPlugins()
        {
            foreach (Plugin p in Plugins)
            {
                try
                {
                    Console.WriteLine($"Try to Disable \"{p.pMeta.Name}\"");
                    p.OnStop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
