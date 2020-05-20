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
        public new string IconURI { get; set; }

        public Achievement ToAchievement() => this;

        public static List<Achievement> FromString(SoraDbContext ctx, string s)
            => s.Split(", ")
                .Select(str => ctx.Achievements.FirstOrDefault(a => a.Name == str))
                .Where(r => r != null)
                .Select(achievement => achievement.ToAchievement())
                .ToList();

        public static Task<DbAchievement> GetAchievement(SoraDbContext ctx, string name)
            => ctx.Achievements.FirstOrDefaultAsync(x => x.Name == name);

        public static async void NewAchievement(SoraDbContext ctx,
            string name, string displayName, string desc, string icon)
        {
            await ctx.Achievements.AddAsync(new DbAchievement
            {
                Name = name,
                Description = desc,
                DisplayName = displayName,
                IconURI = icon
            });

            await ctx.SaveChangesAsync();
        }
    }
}