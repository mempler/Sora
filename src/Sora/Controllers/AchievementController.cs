using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework.Allocation;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/images/medals-client/{achievement}")]
    public class AchievementController : Controller
    {
        private readonly Cache _cache;
        private readonly SoraDbContext _ctx;

        public AchievementController(
            SoraDbContext ctx,
            Cache cache
        )
        {
            _ctx = ctx;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string achievement)
        {
            if (_cache.TryGet($"jibril:achievements:{achievement}", out byte[] res))
                return File(res, "image/png");

            res = (await DbAchievement
                  .GetAchievement(_ctx, achievement.Replace(".png", string.Empty))
                  )?.GetIconImage();

            if (res == null)
                return NotFound();

            _cache.Set("sora:achievements:" + achievement, res, TimeSpan.FromHours(1));
            return File(res, "image/png");
        }
    }
}
