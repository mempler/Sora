using System;
using System.IO;
using Newtonsoft.Json;

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
        public string Hostname;
        public short Port;
        public bool FreeDirect;
    }

    public interface IConfig
    {
        CMySql MySql { get; set; }
        CRedis Redis { get; set; }
        CCheesegull Cheesegull { get; set; }
        CServer Server { get; set; }
    }

    public static class ConfigUtil
    {
        public static T ReadConfig<T>() where T : IConfig, new()
        {
            if (File.Exists("config.json"))
            {
                T r = JsonConvert.DeserializeObject<T>(File.ReadAllText("config.json"));
                File.WriteAllText("config.json",
                                  JsonConvert.SerializeObject(r, Formatting.Indented)); // to Update Config
                return r;
            }

            T cfg = new T();

            cfg.MySql = new CMySql
            {
                Hostname = "localhost",
                Username = "root",
                Password = string.Empty,
                Port     = 3306,
                Database = "Sora"
            };

            cfg.Redis = new CRedis
            {
                Hostname = "localhost"
            };

            cfg.Cheesegull = new CCheesegull
            {
                Hostname = "https://pisstau.be"
            };
            cfg.Server = new CServer
            {
                Hostname = "localhost",
                Port = 4312,
                FreeDirect = true
            };

            File.WriteAllText("config.json", JsonConvert.SerializeObject(cfg, Formatting.Indented));
            Logger.Info("Config has been created! please edit.");
            Environment.Exit(0);
            return cfg;
        }
    }

    public struct CRedis
    {
        public string Hostname;
    }

    public struct CCheesegull
    {
        public string Hostname;
    }

    public struct CMySql
    {
        public string Username;
        public string Password;
        public string Hostname;
        public short Port;
        public string Database;
    }
}
