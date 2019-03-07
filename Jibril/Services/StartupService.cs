using System.IO;
using System.Threading.Tasks;
using EventManager.Services;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril.Services
{
    public class StartupService
    {
        private readonly Database _db;
        private readonly Config _config;
        private readonly PluginService _plugs;
        private readonly EventManager.EventManager _ev;
        private readonly Cache _cache;

        public StartupService(
            Database db,
            Config config,
            PluginService plugs,
            EventManager.EventManager ev,
            Cache cache)
        {
            _db = db;
            _config = config;
            _plugs = plugs;
            _ev = ev;
            _cache = cache;
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
            _ev.RegisterService(_cache);  // Cache System

            _ev.BuildService();
            _ev.RegisterEvents();

            _ev.RegisterService(_config); // Config
            _ev.RegisterService(_db);     // Database
            _ev.RegisterService(_plugs);  // Plugin Service
            _ev.RegisterService(_ev);     // EventManager
            _ev.RegisterService(_cache);  // Cache System

            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");

            foreach (string plug in Directory.GetFiles("plugins"))
                _plugs.LoadPlugin("plugins/" + plug);

            _ev.BuildService();

            return Task.CompletedTask;
        }
    }
}