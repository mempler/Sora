#region LICENSE

/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Sora.Database.Models;
using Sora.Helpers;

namespace Sora.Database
{
    public sealed class SoraDbContext : DbContext
    {
        private static MemoryCache _memoryCache;

        private readonly IMySQLConfig _config;

        public SoraDbContext()
            : this(null)
        {
        }

        public SoraDbContext(ConfigUtil cfgUtil = null, IMySQLConfig config = null)
        {
            if (cfgUtil == null)
            {
                if (_memoryCache == null)
                    _memoryCache = new MemoryCache(
                        new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromDays(365)}
                    );
                cfgUtil = new ConfigUtil(_memoryCache);
            }

            _config = config;
            if (config == null)
                _config = cfgUtil.ReadConfig<MySQLConfig>();
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Users> Users { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Achievements> Achievements { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<UserStats> UserStats { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Scores> Scores { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Friends> Friends { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<LeaderboardStd> LeaderboardStd { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<LeaderboardRx> LeaderboardRx { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Beatmaps> Beatmaps { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<BeatmapSets> BeatmapSets { get; set; }

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
    }
}
