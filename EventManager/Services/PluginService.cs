using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Shared.Helpers;

namespace EventManager.Services
{
    internal class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName) => null;
    }
    
    public class PluginService
    {
        private readonly Dictionary<string, Assembly> _loadedPlugins
            = new Dictionary<string, Assembly>(); 
        
        private readonly Dictionary<Assembly, IPlugin> _entryPoints
            = new Dictionary<Assembly, IPlugin>(); 
        
        private readonly PluginAssemblyLoadContext _context
            = new PluginAssemblyLoadContext();

        private readonly EventManager _ev;
        
        public PluginService(EventManager ev) => _ev = ev;

        public bool LoadPlugin(string filename)
        {
            try
            {
                Assembly asm = _context.LoadFromAssemblyPath(filename);
                _loadedPlugins.Add(filename, asm);
                _ev.LoadAssembly(asm);
                Logger.Info("Loaded new Assembly", asm);
                GetEntryPoint(asm)?.OnEnable();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Err(ex);
                return false;
            }
        }

        public IPlugin GetEntryPoint(Assembly asm)
        {
            if (_entryPoints.ContainsKey(asm))
                return _entryPoints[asm];
            
            Type iplug = asm.GetTypes()
                            .FirstOrDefault(x => x.IsClass &&
                                                 (x.BaseType == typeof(IPlugin) ||
                                                  x.BaseType == typeof(Plugin)));
            
            if (iplug == null) {
                Logger.Warn("No entry point found for plugin", asm);
                _entryPoints.Add(asm, null);
                return null;
            }

            foreach (ConstructorInfo cInfo in iplug.GetConstructors())
                cInfo.Invoke(cInfo.GetParameters()
                                  .Select(prms => _ev.Provider.GetService(prms.ParameterType))
                                  .ToArray());
            
            IPlugin retVal = Activator.CreateInstance(iplug) as IPlugin;

            _entryPoints.Add(asm, retVal);

            return retVal;
        } 

        public bool UnloadPlugin(string filename) => throw new PlatformNotSupportedException("Not supported in this version (.net core 3.0 has support for dynamic unloading)");
    }
}