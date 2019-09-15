using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sora.Enums;

namespace Sora.Database.Models
{
    public interface ILeaderboard
    {
        int Id { get; set; }
        
        ulong RankedScoreOsu { get; set; }
        ulong RankedScoreTaiko { get; set; }
        ulong RankedScoreCtb { get; set; }
        ulong RankedScoreMania { get; set; }
        
        ulong TotalScoreOsu { get; set; }
        ulong TotalScoreTaiko { get; set; }
        ulong TotalScoreCtb { get; set; }
        ulong TotalScoreMania { get; set; }
        
        ulong PlayCountOsu { get; set; }
        ulong PlayCountTaiko { get; set; }
        ulong PlayCountCtb { get; set; }
        ulong PlayCountMania { get; set; }


        double PerformancePointsOsu { get; set; }
        double PerformancePointsTaiko { get; set; }
        double PerformancePointsCtb { get; set; }
        double PerformancePointsMania { get; set; }
    }
    
    public abstract partial class Leaderboard <T> : ILeaderboard
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong RankedScoreMania { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong TotalScoreMania { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public ulong PlayCountMania { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsOsu { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsTaiko { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsCtb { get; set; }

        [Required]
        [DefaultValue(0)]
        public double PerformancePointsMania { get; set; }

        public abstract double GetAccuracy(SoraDbContext ctx, PlayMode mode);
        public abstract void UpdatePP(SoraDbContext ctx, PlayMode mode);
    }
}
