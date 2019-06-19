using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Sora.Helpers
{
    public class Config : IMySQLConfig, IServerConfig, ICheesegullConfig
    {
        public CServer Server { get; set; }
        public CCheesegull Cheesegull { get; set; }
        public CMySql MySql { get; set; }
    }

    public interface IMySQLConfig : IConfig
    {
        CMySql MySql { get; set; } 
    }

    public interface IServerConfig : IConfig
    {
        CServer Server { get; set; }
    }

    public interface ICheesegullConfig : IConfig
    {
        CCheesegull Cheesegull { get; set; }
    }

    public interface IConfig
    {
    }

    public class ConfigUtil
    {
        private readonly IMemoryCache _cache;

        public ConfigUtil(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        public T ReadConfig<T>(string cfgName = "config.json", T defaultConfig = default) where T : IConfig, new()
        {
            T c;
            if ((c = _cache.Get<T>(cfgName)) != null)
                return c;
            
            if (File.Exists(cfgName))
            {
                _cache.Set(cfgName, c = JsonConvert.DeserializeObject<T>(File.ReadAllText(cfgName)),
                           TimeSpan.FromDays(365));
                return c;
            }
            
            c = defaultConfig;
            
            if (c == null)
                c = new T();

            File.WriteAllText(cfgName, JsonConvert.SerializeObject(c, Formatting.Indented));
            Logger.Info($"Config {cfgName} has been created! please edit.");
            Environment.Exit(0);
            
            return c;
        }
    }

    public struct CCheesegull
    {
        public string URI;
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
        public bool FreeDirect;
    }
}
