using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Shared.Database.Models
{
    public class LeaderboardRx
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] [DefaultValue(0)]
        public ulong RankedScoreStd { get; set; }
        [Required] [DefaultValue(0)]
        public ulong RankedScoreTaiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong RankedScoreCtb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong RankedScoreMania { get; set; }

        [Required] [DefaultValue(0)]
        public ulong TotalScoreStd { get; set; }
        [Required] [DefaultValue(0)]
        public ulong TotalScoreTaiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong TotalScoreCtb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong TotalScoreMania { get; set; }

        [Required] [DefaultValue(0)]
        public ulong Count300Std { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count300Taiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count300Ctb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count300Mania { get; set; }

        
        [Required] [DefaultValue(0)]
        public ulong Count100Std { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count100Taiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count100Ctb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count100Mania { get; set; }

                
        [Required] [DefaultValue(0)]
        public ulong Count50Std { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count50Taiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count50Ctb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong Count50Mania { get; set; }

                
        [Required] [DefaultValue(0)]
        public ulong CountMissStd { get; set; }
        [Required] [DefaultValue(0)]
        public ulong CountMissTaiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong CountMissCtb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong CountMissMania { get; set; }

        [Required] [DefaultValue(0)]
        public ulong PlayCountStd { get; set; }
        [Required] [DefaultValue(0)]
        public ulong PlayCountTaiko { get; set; }
        [Required] [DefaultValue(0)]
        public ulong PlayCountCtb { get; set; }
        [Required] [DefaultValue(0)]
        public ulong PlayCountMania { get; set; }

        [Required] [DefaultValue(0)]
        public double PeppyPointsStd { get; set; }
        [Required] [DefaultValue(0)]
        public double PeppyPointsTaiko { get; set; }
        [Required] [DefaultValue(0)]
        public double PeppyPointsCtb { get; set; }
        [Required] [DefaultValue(0)]
        public double PeppyPointsMania { get; set; }

        public static LeaderboardRx GetLeaderboard(int userId)
        {
            using (var db = new SoraContext())
            {
                var result = db.LeaderboardRx.Where(t => t.Id == userId).Select(e => e).FirstOrDefault();
                if (result == null) db.LeaderboardRx.Add(new LeaderboardRx {Id = userId});
                return result ?? new LeaderboardRx {Id = userId};
            }
        }

        public static LeaderboardRx GetLeaderboard(Users user) => GetLeaderboard(user.Id);
    }
}
