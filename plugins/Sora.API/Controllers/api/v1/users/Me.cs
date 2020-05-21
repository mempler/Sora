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
        private readonly SoraDbContext _context;

        public Me(ILogger<Me> logger, SoraDbContext context)
        {
            _logger = logger;
            _context = context;
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

            var user = await DbUser.GetDbUser(_context, int.Parse(User.Identity.Name));
            if (user == null)
                return Ok(new
                {
                    authentication = "basic"
                });

            var lb = await DbLeaderboard.GetLeaderboardAsync(_context, user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                Permissions = Permission.From(user.Permissions).Perms,
                user.Status,
                user.StatusUntil,
                user.StatusReason,
                Achievements = DbAchievement.FromString(_context, user.Achievements ?? ""),
                Country = "XX",
                globalRank = lb.GetPosition(_context, PlayMode.Osu),
                Accuracy = lb.GetAccuracy(_context, PlayMode.Osu),
                Performance = lb.PerformancePointsOsu,
                TotalScore = lb.TotalScoreOsu,
                RankedScore = lb.RankedScoreOsu,
                PlayCount = lb.PlayCountOsu
            });
        }
    }
}