#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Sora.Helpers;

namespace Sora.Services
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

            object[] tArgs = (from cInfo in iplug.GetConstructors()
                              from pInfo in cInfo.GetParameters()
                              select _ev.Provider.GetService(pInfo.ParameterType)).ToArray();

            if (tArgs.Any(x => x == null))
                throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");
            
            IPlugin retVal = Activator.CreateInstance(iplug, tArgs) as IPlugin;

            _entryPoints.Add(asm, retVal);

            return retVal;
        } 

        public bool UnloadPlugin(string filename) => throw new PlatformNotSupportedException("Not supported in this version (.net core 3.0 has support for dynamic unloading)");
    }
}