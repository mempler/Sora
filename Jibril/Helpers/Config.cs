using Shared.Helpers;

namespace Jibril.Helpers
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
        public string Hostname;
        public string Cheesegull;
    }
}