using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Database.Models
{
    [Flags]
    public enum DbBeatmapFlags
    {
        None = 1 << 0,
        RankedFreeze = 1 << 1
    }
    
    [Table("Beatmaps")]
    public class DbBeatmap
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SetId { get; set; }

        [Required]
        public Pisstaube.RankedStatus RankedStatus { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(32)")]
        public string FileMd5 { get; set; }

        [Required]
        public PlayMode PlayMode { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Artist { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string DiffName { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string FileName { get; set; }

        [Required]
        public DbBeatmapFlags Flags { get; set; }
        
        public static DbBeatmap FromBeatmap(Beatmap beatmap, BeatmapSet parent)
        {
            return new DbBeatmap
            {
                Id = beatmap.BeatmapID,
                SetId = parent.SetID,
                RankedStatus = parent.RankedStatus,
                FileMd5 = beatmap.FileMD5,
                PlayMode = beatmap.Mode,
                Artist = parent.Artist,
                Title = parent.Title,
                DiffName = beatmap.DiffName,
                FileName = $"{parent.Artist} - {parent.Title} ({parent.Creator}) [{beatmap.DiffName}].osu",
                Flags = DbBeatmapFlags.None
            };
        }
        
        public static IEnumerable<DbBeatmap> FromBeatmapSet(BeatmapSet set) =>
            set.ChildrenBeatmaps.Select(beatmap => FromBeatmap(beatmap, set));

        public static DbBeatmap GetBeatmap(SoraDbContext ctx, string fileMd5)
            => ctx
                .Beatmaps
                .FirstOrDefault(t => t.FileMd5 == fileMd5);
    }
}