using EventManager;
using Shared.Helpers;

namespace ExamplePlugin
{
    public class PluginEntry : Plugin
    {
        public override void OnEnable()
        {
            Logger.Info("Startup ExamplePlugin");
        }

        public override void OnDisable()
        {
            Logger.Info("Shutdown ExamplePlugin");
        }
    }
}