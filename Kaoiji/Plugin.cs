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
        public abstract void OnStart();
        public abstract void OnStop();
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
