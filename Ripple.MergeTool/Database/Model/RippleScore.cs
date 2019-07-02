using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sora.Enums;

namespace Ripple.MergeTool.Database.Model
{
    [Table("scores")]
    public class RippleScore
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string beatmap_md5 { get; set; }

        [Required]
        public int userid { get; set; }

        [Required]
        public int score { get; set; }

        [Required]
        public int max_combo { get; set; }

        [Required]
        public int mods { get; set; }

        [Required]
        [Column("300_count")]
        public int C300 { get; set; }

        [Required]
        [Column("100_count")]
        public int C100 { get; set; }

        [Required]
        [Column("50_count")]
        public int C50 { get; set; }

        [Required]
        [Column("gekis_count")]
        public int CGeki { get; set; }

        [Required]
        [Column("katus_count")]
        public int CKatu { get; set; }

        [Required]
        [Column("misses_count")]
        public int CMiss { get; set; }

        [Required]
        public string time { get; set; } // Why is that a varchar.... Ripple ?!?

        [Required]
        public sbyte play_mode { get; set; }

        [Required]
        public double accuracy { get; set; }
    }
}
