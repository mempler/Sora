using Sora.Framework.Utilities;

namespace Sora
{
    public interface IMySqlConfig : IConfig
    {
        CMySql MySql { get; set; }
    }

    public interface IServerConfig : IConfig
    {
        CServer Server { get; set; }
    }

    public struct CMySql
    {
        public string Hostname;
        public short Port;

        public string Username;
        public string Password;
        public string Database;
    }

    public struct CServer
    {
        public string Hostname;
        public short Port;
        public short IrcPort;

        public string ScreenShotHostname;
        public bool FreeDirect;
    }
    
    public class Config : IServerConfig, IMySqlConfig, IPisstaubeConfig
    {
        public CServer Server { get; set; }
        public CMySql MySql { get; set; }
        public CPisstaube Pisstaube { get; set; }
        
        public string Esc { get; set; }
    }
}