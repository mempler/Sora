using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Sora.Database.Models;
using Sora.Helpers;

namespace Sora.Database
{
    public sealed class SoraContext : DbContext
    {
        public DbSet<Users> Users { get; }
        private static readonly bool[] Migrated = { false };

        public SoraContext()
        {
            if (Migrated[0]) return;
            lock ( Migrated )
                if ( !Migrated[0] )
                {
                    Database.Migrate();
                    Migrated[0] = true;
                }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cfg = Config.ReadConfig();
            if (cfg.MySql.Hostname == null)
                Environment.Exit(1);
            optionsBuilder.UseMySql(
                $"Server={cfg.MySql.Hostname};Database={cfg.MySql.Database};User={cfg.MySql.Username};Password={cfg.MySql.Password};Port={cfg.MySql.Port};",
                mysqlOptions => { mysqlOptions.ServerVersion(new Version(10, 2, 15), ServerType.MariaDb); }
            );
        }
    }
}
