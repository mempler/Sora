using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework;
using Sora.Framework.Enums;

namespace Sora.API.Controllers.api.v1.users
{
    [Route("/")]
    [ApiController]
    [AllowAnonymous]
    public class User : Controller
    {
        private readonly SoraDbContextFactory _factory;

        public User(SoraDbContextFactory factory)
        {
            _factory = factory;
        }

        [AllowAnonymous]
        [DisableCors]
        [HttpGet("/api/v1/users/{userId:int}")]
        public async Task<ActionResult> Get(int userId)
        {
            var user = await DbUser.GetDbUser(_factory, userId);
            if (user == null)
                return NotFound(new
                {
                    code = 410,
                    message = "User not found!"
                });

            var lb = await DbLeaderboard.GetLeaderboardAsync(_factory, user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                Permissions = Permission.From(user.Permissions).Perms,
                user.Status,
                user.StatusUntil,
                user.StatusReason,
                Achievements = DbAchievement.FromString(_factory, user.Achievements ?? ""),
                Country = "XX",
                globalRank = lb.GetPosition(_factory, PlayMode.Osu),
                Accuracy = lb.GetAccuracy(_factory, PlayMode.Osu),
                Performance = lb.PerformancePointsOsu,
                TotalScore = lb.TotalScoreOsu,
                RankedScore = lb.RankedScoreOsu,
                PlayCount = lb.PlayCountOsu
            });
        }

        [DisableCors]
        [HttpGet("/api/v1/users/{userName}")]
        public async Task<ActionResult> Get(string userName)
        {
            var user = await DbUser.GetDbUser(_factory, userName);
            if (user == null)
                return NotFound(new
                {
                    code = 410,
                    message = "User not found!"
                });

            var lb = await DbLeaderboard.GetLeaderboardAsync(_factory, user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                Permissions = Permission.From(user.Permissions).Perms,
                user.Status,
                user.StatusUntil,
                user.StatusReason,
                Achievements = DbAchievement.FromString(_factory, user.Achievements ?? ""),
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