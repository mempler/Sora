namespace Shared.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

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

        public Beatmaps GetBeatmap(string fileMD5) => Beatmaps?.FirstOrDefault(b => b.FileMD5 == fileMD5);

        public static BeatmapSets GetBeatmapSet(int setId)
        {
            using (SoraContext db = new SoraContext()) {
                BeatmapSets sts = db.BeatmapSets.FirstOrDefault(s => s.Id == setId);

                if (sts != null)
                    sts.Beatmaps = db.Beatmaps.Where(s => s.BeatmapSetId == sts.Id).ToList();
                
                return sts;
            }
        }
    }
}
