using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework.Enums;

namespace Sora.API.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/get_top_leaderboard")] // /api/v1/get_top_leaderboard
    public class Leaderboard : Controller
    {
        private SoraDbContextFactory _factory;

        public Leaderboard(SoraDbContextFactory factory)
        {
            _factory = factory;
        }

        public struct LBResponse
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public double Performance { get; set; }
            public double Accuracy { get; set; }
            public ulong TotalScore { get; set; }
            public ulong RankedScore { get; set; }
            public string Country { get; set; }
            public ulong PlayCount { get; set; }
        }
        
        [HttpGet]
        [DisableCors]
        public async Task<ActionResult> Get([FromQuery] int offset = 0, [FromQuery] PlayMode mode =
            PlayMode.Osu)
        {
            var lb = (await _factory.Get().Leaderboard
                                   .Where(x => x.Owner.Status != UserStatusFlags.Suspended ||
                                               x.Owner.Status != UserStatusFlags.Restricted)
                                   .OrderByDescending(x => x.PerformancePointsOsu)
                                   .Skip(offset)
                                   .Take(50)
                                   .ToListAsync())
                                   .Select(l => 
                                       new LBResponse
                                       {
                                           Id = l.OwnerId,
                                           Username = DbUser.GetDbUser(_factory, l.OwnerId).Result.UserName,
                                           Accuracy = l.GetAccuracy(_factory, mode),
                                           Performance = mode switch
                                           {
                                               PlayMode.Osu => l.PerformancePointsOsu,
                                               PlayMode.Taiko => l.PerformancePointsTaiko,
                                               PlayMode.Ctb => l.PerformancePointsCtb,
                                               PlayMode.Mania => l.PerformancePointsMania,
                                               _ => 0
                                           },
                                           TotalScore = mode switch
                                           {
                                               PlayMode.Osu => l.TotalScoreOsu,
                                               PlayMode.Taiko => l.TotalScoreTaiko,
                                               PlayMode.Ctb => l.TotalScoreCtb,
                                               PlayMode.Mania => l.TotalScoreMania,
                                               _ => (ulong) 0
                                           },
                                           RankedScore = mode switch
                                           {
                                               PlayMode.Osu => l.RankedScoreOsu,
                                               PlayMode.Taiko => l.RankedScoreTaiko,
                                               PlayMode.Ctb => l.RankedScoreCtb,
                                               PlayMode.Mania => l.RankedScoreMania,
                                               _ => (ulong) 0
                                           },
                                           PlayCount = mode switch
                                           {
                                               PlayMode.Osu => l.PlayCountOsu,
                                               PlayMode.Taiko => l.PlayCountTaiko,
                                               PlayMode.Ctb => l.PlayCountCtb,
                                               PlayMode.Mania => l.PlayCountMania,
                                               _ => (ulong) 0
                                           },
                                           Country = "XX"
                                       }
                                   );
            return Ok(JsonConvert.SerializeObject(lb));
        }
    }
}
