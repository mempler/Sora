using Shared.Helpers;

namespace Sora.Helpers
{
    public class Config : IConfig
    {
        public Shared.Helpers.MySql MySql { get; set; }
        public Redis Redis { get; set; }
        public Server Server { get; set; }
    }

    public struct Server
    {
        public short Port;
        public bool FreeDirect;
    }
}
