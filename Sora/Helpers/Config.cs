using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Sora.Helpers
{
    internal static class Config
    {
        public static CFG ReadConfig()
        {
            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new CFG(), Formatting.Indented));
            return JsonConvert.DeserializeObject<CFG>(File.ReadAllText("config.json"));
        }
    }

    internal class CFG
    {
        public Server Server = new Server() { Port=5001 };
        public MySql MySql = new MySql() { Database = "shiro", Hostname = "127.0.0.1", Username = "root", Port = 3306, Password = "" };
    }
    struct Server
    {
        public short Port;
    }
    struct MySql
    {
        public string Username;
        public string Password;
        public string Hostname;
        public short Port;
        public string Database;
    }
}
