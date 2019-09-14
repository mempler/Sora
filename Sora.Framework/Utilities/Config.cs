using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Sora.Framework.Utilities
{
    public interface IPisstaubeConfig : IConfig
    {
        CPisstaube Pisstaube { get; set; }
    }

    public interface IConfig
    {
    }

    public class ConfigUtil
    {
        private readonly IMemoryCache _cache;

        public ConfigUtil(IMemoryCache cache) => _cache = cache;

        public bool TryReadConfig<T>(out T c, string cfgName = "config.json", T defaultConfig = default)
            where T : class, IConfig, new()
        {
            if (_cache.TryGetValue(cfgName, out c))
                return true;

            if (File.Exists(cfgName))
            {
                _cache.Set(
                    cfgName, c = JsonConvert.DeserializeObject<T>(File.ReadAllText(cfgName)),
                    TimeSpan.FromDays(365)
                );
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
