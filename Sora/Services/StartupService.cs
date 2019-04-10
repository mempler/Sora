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

using System.IO;
using System.Threading.Tasks;
using EventManager.Services;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;
using Sora.Helpers;
using Sora.Objects;

namespace Sora.Services
{
    public class StartupService
    {
        private readonly Database _db;
        private readonly Config _config;
        private readonly IConfig _icfg;
        private readonly PluginService _plugs;
        private readonly EventManager.EventManager _ev;
        private readonly MultiplayerService _mps;
        private readonly PacketStreamService _pss;
        private readonly ChannelService _cs;
        private readonly PresenceService _ps;

        public StartupService(
            Database db,
            Config config,
            IConfig icfg,
            PluginService plugs,
            EventManager.EventManager ev,

            MultiplayerService mps,
            PacketStreamService pss,
            ChannelService cs,
            PresenceService ps
            )
        {
            _db = db;
            _config = config;
            _icfg = icfg;
            _plugs = plugs;
            _ev = ev;
            _mps = mps;
            _pss = pss;
            _cs = cs;
            _ps = ps;
        }

        public Task Start()
        {
            // Create Sora (bot) if not exists.
            if (Users.GetUser(_db, 100) == null)
                Users.InsertUser(_db, new Users
                {
                    Id         = 100,
                    Username   = "Sora",
                    Email      = "bot@gigamons.de",
                    Password   = "",
                    Privileges = 0
                });
            
            Localisation.Initialize();

            _ev.RegisterService(_config); // Config
            _ev.RegisterService(_db);     // Database
            _ev.RegisterService(_plugs);  // Plugin Service
            _ev.RegisterService(_ev);     // EventManager
            _ev.RegisterService(_mps);    // Multiplayer Service
            _ev.RegisterService(_pss);    // PacketStream Service
            _ev.RegisterService(_cs);     // Channel Service
            _ev.RegisterService(_ps);     // Presence Service
            _ev.RegisterService(_icfg);   // Interface Config for Shared
            
            _ev.BuildService();
            _ev.RegisterEvents();

            _ev.RegisterService(_config); // Config
            _ev.RegisterService(_db);     // Database
            _ev.RegisterService(_plugs);  // Plugin Service
            _ev.RegisterService(_ev);     // EventManager
            _ev.RegisterService(_mps);    // Multiplayer Service
            _ev.RegisterService(_pss);    // PacketStream Service
            _ev.RegisterService(_cs);     // Channel Service
            _ev.RegisterService(_ps);     // Presence Service
            _ev.RegisterService(_icfg); // Interface Config for Shared
                        
            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");
            
            foreach (string plug in Directory.GetFiles("plugins"))
                _plugs.LoadPlugin("plugins/" + plug);

            _ev.BuildService();
            
            return Task.CompletedTask;
        }
    }
}