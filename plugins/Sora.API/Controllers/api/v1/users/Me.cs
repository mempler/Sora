using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework;
using Sora.Framework.Enums;

namespace Sora.API.Controllers.api.v1.users
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/me")] // /api/v1/me
    public class Me : Controller
    {
        private readonly ILogger<Me> _logger;
        private readonly SoraDbContextFactory _factory;

        public Me(ILogger<Me> logger, SoraDbContextFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }
        
        [HttpGet]
        [Authorize]
        [DisableCors]
        public async Task<ActionResult> Get()
        {
            if (!User.Identity.IsAuthenticated)
                return Ok(new
                {
                    authentication = "basic"
                });

            var user = await DBUser.GetDBUser(_factory, int.Parse(User.Identity.Name));
            if (user == null)
                return Ok(new
                {
                    authentication = "basic"
                });

            var lb = await DBLeaderboard.GetLeaderboardAsync(_factory, user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                Permissions = Permission.From(user.Permissions).Perms,
                user.Status,
                user.StatusUntil,
                user.StatusReason,
                Achievements = DBAchievement.FromString(_factory, user.Achievements ?? ""),
                Country = "XX",
                globalRank = lb.GetPosition(_factory, PlayMode.Osu),
                Accuracy = lb.GetAccuracy(_factory, PlayMode.Osu),
                Performance = lb.PerformancePointsOsu,
                TotalScore = lb.TotalScoreOsu,
                RankedScore = lb.RankedScoreOsu,
                PlayCount = lb.PlayCountOsu
            });
        }
    }
}