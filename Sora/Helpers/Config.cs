using Shared.Helpers;

namespace Sora.Helpers
{
    public class Config : IConfig
    {
        public CMySql MySql { get; set; }
        public CRedis Redis { get; set; }
        public CCheesegull Cheesegull { get; set; }
        public CServer Server { get; set; }
    }

    public struct CServer
    {
        public short Port;
        public bool FreeDirect;
    }
}
