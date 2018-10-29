namespace Shared.Database.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Enums;

    public class Beatmaps
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [DefaultValue(RankedStatus.LatestPending)]
        public RankedStatus RankedStatus { get; set; }

        public DateTime RankedDate { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        [DefaultValue("Nobody")]
        public string Artist { get; set; }

        [Required]
        public int BeatmapSetId { get; set; }
        
        [ForeignKey("BeatmapSetId")]
        public BeatmapSets BeatmapSet { get; set; }

        [Required]
        [DefaultValue(0)]
        public float Bpm { get; set; }

        [Required]
        [DefaultValue("Nobody")]
        public string BeatmapCreator { get; set; }

        [Required]
        [DefaultValue(0)]
        public int BeatmapCreatorId { get; set; }

        [Required]
        [DefaultValue(0d)]
        public double Difficulty { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Cs { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Od { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Ar { get; set; }

        [Required]
        [DefaultValue(0f)]
        public float Hp { get; set; }

        [Required]
        [DefaultValue(0f)]
        public int HitLength { get; set; }

        [Required]
        [DefaultValue("")]
        public string Title { get; set; }

        [Required]
        [DefaultValue(0)]
        public int TotalLength { get; set; }

        [Required]
        [DefaultValue("")]
        public string DifficultyName { get; set; }

        [Required]
        public string FileMD5 { get; set; }

        [Required]
        [DefaultValue(PlayMode.Osu)]
        public PlayMode PlayMode { get; set; }

        [Required]
        [DefaultValue("")]
        public string Tags { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PlayCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PassCount { get; set; }
    }
}
