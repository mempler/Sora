using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaoiji
{
    public abstract class Plugin
    {
        public Plugin() { }
        public PluginMeta pMeta;
        public virtual void OnStart() { Console.WriteLine($"[{pMeta.Name}] Enabled!"); }
        public virtual void OnStop() { Console.WriteLine($"[{pMeta.Name}] Disabled!"); }
    }
    public class PluginMeta
    {
        public string Name;
        public string Description;
        public double Version;
        public string Plugin;
        public string[] Dependencies;
    }
}
