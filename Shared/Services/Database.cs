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
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Services
{
    public sealed class Database : DbContext
    {
        private readonly IConfig _config;
        private static readonly bool[] Migrated = {false};

        public Database(IConfig config)
        {
            _config = config;
            
            if (Migrated[0]) return;
            lock (Migrated)
            {
                if (Migrated[0]) return;
                Database.Migrate();
                Migrated[0] = true;
            }
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Users> Users { get; set; }

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_config.MySql.Hostname == null)
                Environment.Exit(1);
            optionsBuilder.UseMySql(
                $"Server={_config.MySql.Hostname};Database={_config.MySql.Database};User={_config.MySql.Username};Password={_config.MySql.Password};Port={_config.MySql.Port};CharSet=utf8mb4;",
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