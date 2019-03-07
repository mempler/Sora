using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Shared.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Shared.Models
{
    public class BeatmapSets
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [NotMapped]
        public List<Beatmaps> Beatmaps { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        [DefaultValue(0)]
        public int FavouriteCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PlayCount { get; set; }

        [Required]
        [DefaultValue(0)]
        public int PassCount { get; set; }

        public Beatmaps GetBeatmap(string fileMd5) => Beatmaps?.FirstOrDefault(b => b.FileMd5 == fileMd5);

        public static BeatmapSets GetBeatmapSet(Database db, int setId)
        {
            BeatmapSets sts = db.BeatmapSets.FirstOrDefault(s => s.Id == setId);

            if (sts != null)
                sts.Beatmaps = db.Beatmaps.Where(s => s.BeatmapSetId == sts.Id).ToList();

            return sts;
        }
    }
}