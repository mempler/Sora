using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Ripple.MergeTool.Database.Model;
using Sora.Helpers;

namespace Ripple.MergeTool.Database
{
    public class RippleDbContext : DbContext
    {
        public DbSet<RippleUsers> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Config config = ConfigUtil.ReadConfig<Config>("ripple.config.json");
            
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
    }
}