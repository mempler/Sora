using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Sora.Framework.Utilities
{
    public interface IPisstaubeConfig : IConfig
    {
        CPisstaube Pisstaube { get; set; }
    }

    public interface IConfig
    {
    }

    public static class ConfigUtil
    {
        public static bool TryReadConfig<T>(out T c, string cfgName = "config.json", T defaultConfig = default)
            where T : class, IConfig, new()
        {
            if (File.Exists(cfgName))
            {
                c = JsonConvert.DeserializeObject<T>(File.ReadAllText(cfgName));
                return true;
            }

            c = defaultConfig ?? new T();

            File.WriteAllText(cfgName, JsonConvert.SerializeObject(c, Formatting.Indented));
            Logger.Info($"Config {cfgName} has been created! please edit.");
            return false;
        }
    }

    public struct CPisstaube
    {
        public string URI;
    }
}
