using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sora.Framework.Objects;

namespace Sora.Database.Models
{
    [Table("Achievements")]
    [UsedImplicitly]
    public class DbAchievement : Achievement
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new string Name { get; set; }

        [Required]
        public new string DisplayName { get; set; }

        [Required]
        public new string Description { get; set; }

        [Required]
        public new string IconUri { get; set; }

        public Achievement ToAchievement() => this;

        public static List<Achievement> FromString(SoraDbContextFactory factory, string s)
            => s.Split(", ")
                .Select(str => factory.Get().Achievements.FirstOrDefault(a => a.Name == str))
                .Where(r => r != null)
                .Select(achievement => achievement.ToAchievement())
                .ToList();

        public static Task<DbAchievement> GetAchievement(SoraDbContextFactory factory, string name)
            => factory.Get().Achievements.FirstOrDefaultAsync(x => x.Name == name);

        public static async void NewAchievement(SoraDbContextFactory factory,
            string name, string displayName, string desc, string icon)
        {
            using var db = factory.GetForWrite();

            await db.Context.Achievements.AddAsync(new DbAchievement
            {
                Name = name,
                Description = desc,
                DisplayName = displayName,
                IconUri = icon
            });
        }
    }
}