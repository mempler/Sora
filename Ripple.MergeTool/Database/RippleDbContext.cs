using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Ripple.MergeTool.Database.Model;
using Sora.Helpers;

namespace Ripple.MergeTool.Database
{
    public class RippleDbContext : DbContext
    {
        private static MemoryCache _memoryCache;

        public DbSet<RippleBeatmap> Beatmaps { get; set; }
        public DbSet<RippleUser> Users { get; set; }
        public DbSet<RippleScore> Scores { get; set; }
        public DbSet<RippleFriends> Friends { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_memoryCache == null)
                _memoryCache = new MemoryCache(
                    new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromDays(365)}
                );
            var cfgUtil = new ConfigUtil(_memoryCache);

            var config = cfgUtil.ReadConfig<MySQLConfig>("ripple.config.json");

            if (config.MySql.Hostname == null)
                Logger.Fatal("MySQL Hostname cannot be null!");

            optionsBuilder.UseMySql(
                $"Server={config.MySql.Hostname};" +
                $"Database={config.MySql.Database};" +
                $"User={config.MySql.Username};" +
                $"Password={config.MySql.Password};" +
                $"Port={config.MySql.Port};CharSet=utf8mb4;",
                mysqlOptions =>
                {
                    mysqlOptions.ServerVersion(new Version(10, 2, 15), ServerType.MariaDb);
                    mysqlOptions.UnicodeCharSet(CharSet.Utf8mb4);
                    mysqlOptions.AnsiCharSet(CharSet.Utf8mb4);
                }
            );
        }

        private class MySQLConfig : IMySQLConfig
        {
            public CMySql MySql { get; set; }
        }

        public struct CRipple
        {
            public string LetsBeatmapPath;
            public string LetsReplaysPath;
            public string LetsScreenshotPath;
        }

        public class RippleConfig : IMySQLConfig, ICheesegullConfig
        {
            public string SoraDataDirectory { get; set; }

            public CRipple CRipple { get; set; }
            public CCheesegull Cheesegull { get; set; }
            public CMySql MySql { get; set; }
        }
    }
}
