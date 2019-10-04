using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Sora.API
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
            _logger.LogInformation("Startup Sora Web API");
        }

        public override void OnDisable()
        {
            _logger.LogInformation("Shutdown olSora Web API");
        }
    }
}
