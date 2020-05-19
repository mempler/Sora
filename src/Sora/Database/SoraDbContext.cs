using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Sora.Database.Models;
using Sora.Framework.Utilities;

namespace Sora.Database
{
    public sealed class SoraDbContext : DbContext
    {
        private readonly IMySqlConfig _config;

        public SoraDbContext()
            : this(null)
        {
        }

        public SoraDbContext(IMySqlConfig config = null)
        {
            _config = config;
            if (config != null) return;
            
            if (ConfigUtil.TryReadConfig(out MySqlConfig cfg))
                _config = cfg;
        }
        
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbFriend> Friends { get; set; }
        public DbSet<DbAchievement> Achievements { get; set; }
        public DbSet<DbScore> Scores { get; set; }
        public DbSet<DbLeaderboard> Leaderboard { get; set; }
        public DbSet<DboAuthClient> OAuthClients { get; set; }

        public void Migrate()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_config.MySql.Hostname == null)
                Logger.Fatal("MySQL Hostname cannot be null!");

            optionsBuilder.UseMySql(
                $"Server={_config.MySql.Hostname};" +
                $"Database={_config.MySql.Database};" +
                $"User={_config.MySql.Username};" +
                $"Password={_config.MySql.Password};" +
                $"Port={_config.MySql.Port};CharSet=utf8mb4;",
                mysqlOptions =>
                {
                    mysqlOptions.CommandTimeout(int.MaxValue);
                    
                    mysqlOptions.ServerVersion(new Version(10, 2, 15), ServerType.MariaDb);
                    mysqlOptions.CharSet(CharSet.Utf8Mb4);
                }
            );
        }

        private class MySqlConfig : IMySqlConfig
        {
            public CMySql MySql { get; set; }
        }
    }
}
