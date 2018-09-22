using System.IO;
using Newtonsoft.Json;

namespace Shared.Helpers
{
    internal static class Config
    {
        public static Cfg ReadConfig()
        {
            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Cfg(), Formatting.Indented));
            return JsonConvert.DeserializeObject<Cfg>(File.ReadAllText("config.json"));
        }
    }

    internal class Cfg
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
