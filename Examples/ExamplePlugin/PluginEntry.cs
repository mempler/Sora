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

using Microsoft.Extensions.Logging;
using Sora;
using Sora.Framework.Utilities;

namespace ExamplePlugin
{
    public class PluginEntry : Plugin
    {
        private readonly ILogger<PluginEntry> _logger;

        public PluginEntry(ILogger<PluginEntry> logger)
        {
            _logger = logger;
        }
        
        public override void OnEnable()
        {
            _logger.LogInformation("Startup ExamplePlugin");
        }

        public override void OnDisable()
        {
            _logger.LogInformation("Shutdown ExamplePlugin");
        }
    }
}