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
        private readonly SoraDbContextFactory _factory;

        public AchievementController(
            SoraDbContextFactory factory,
            Cache cache
        )
        {
            _factory = factory;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string achievement)
        {
            if (_cache.TryGet($"jibril:achievements:{achievement}", out byte[] res))
                return File(res, "image/png");

            res = (await DBAchievement
                  .GetAchievement(_factory, achievement.Replace(".png", string.Empty))
                  )?.GetIconImage();

            if (res == null)
                return NotFound();

            _cache.Set("sora:achievements:" + achievement, res, TimeSpan.FromHours(1));
            return File(res, "image/png");
        }
    }
}
