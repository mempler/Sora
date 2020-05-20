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

        public SoraDbContext(DbContextOptions<SoraDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbFriend> Friends { get; set; }
        public DbSet<DbAchievement> Achievements { get; set; }
        public DbSet<DbScore> Scores { get; set; }
        public DbSet<DbLeaderboard> Leaderboard { get; set; }
        public DbSet<DboAuthClient> OAuthClients { get; set; }
        public DbSet<DbBeatmap> Beatmaps { get; set; }

        public void Migrate()
        {
            Database.Migrate();
        }
        
        private class MySqlConfig : IMySqlConfig
        {
            public CMySql MySql { get; set; }
        }
    }
}
