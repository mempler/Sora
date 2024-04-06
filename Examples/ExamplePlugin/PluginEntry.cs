using Microsoft.AspNetCore.Builder;
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
        
        public override void OnEnable(IApplicationBuilder app)
        {
            _logger.LogInformation("Startup ExamplePlugin");
        }

        public override void OnDisable()
        {
            _logger.LogInformation("Shutdown ExamplePlugin");
        }
    }
}