#region LICENSE

/*
    olSora - A Modular Bancho written in C#
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
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;
using Sora.Framework.Utilities;

namespace Sora.Services
{
    internal class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName) => null;
    }

    public class PluginService
    {
        private readonly PluginAssemblyLoadContext _context
            = new PluginAssemblyLoadContext();

        private readonly Dictionary<Assembly, IPlugin> _entryPoints
            = new Dictionary<Assembly, IPlugin>();

        private readonly EventManager _ev;
        private readonly ApplicationPartManager _appPartManager;
        private readonly ILogger<PluginService> _logger;

        private readonly Dictionary<string, Assembly> _loadedPlugins
            = new Dictionary<string, Assembly>();

        public PluginService(EventManager ev, ApplicationPartManager appPartManager,
            ILogger<PluginService> logger)
        {
            _ev = ev;
            _appPartManager = appPartManager;
            _logger = logger;
        }

        public bool LoadPlugin(string filename)
        {
            try
            {
                var asm = _context.LoadFromAssemblyPath(filename);
                if (asm == null) return false;
                
                _loadedPlugins.Add(filename, asm);
                _ev.LoadAssembly(asm);
                _logger.LogInformation("Loaded new Assembly {asm}", asm);

                // Register ASP.NET Core Controllers
                _appPartManager.ApplicationParts.Add(new AssemblyPart(asm));
                
                GetEntryPoint(asm)?.OnEnable();
                
                return true;
            } catch (Exception ex)
            {
                Logger.Err(ex);
                return false;
            }
        }

        public IPlugin GetEntryPoint(Assembly asm)
        {
            if (_entryPoints.ContainsKey(asm))
                return _entryPoints[asm];

            var iplug = asm.GetTypes()
                           .FirstOrDefault(
                               x => x.IsClass &&
                                    (x.BaseType == typeof(IPlugin) ||
                                     x.BaseType == typeof(Plugin))
                           );

            if (iplug == null)
            {
                Logger.Warn("No entry point found for plugin", asm);
                _entryPoints.Add(asm, null);
                return null;
            }

            var tArgs = (from cInfo in iplug.GetConstructors()
                         from pInfo in cInfo.GetParameters()
                         select _ev.Provider.GetService(pInfo.ParameterType)).ToArray();

            if (tArgs.Any(x => x == null))
                throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");

            var retVal = Activator.CreateInstance(iplug, tArgs) as IPlugin;

            _entryPoints.Add(asm, retVal);

            return retVal;
        }

        public bool UnloadPlugins()
        {
            try
            {
                _context.Unload();
                return true;
            } catch (Exception ex)
            {
                Logger.Err(ex);
                return false;
            }
        }
    }
}
