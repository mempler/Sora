using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sora.Enums;

namespace Ripple.MergeTool.Database.Model
{
    [Table("beatmaps")]
    public class RippleBeatmap
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int beatmap_id { get; set; }

        [Required]
        public RankedStatus ranked { get; set; }
    }
}
