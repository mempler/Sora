using Microsoft.AspNetCore.Mvc;
using Sora.Database;
using Sora.Database.Models;
using Sora.Helpers;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/images/medals-client/{achievement}")]
    public class AchievementController : Controller
    {
        private readonly SoraDbContextFactory _factory;
        private readonly Cache _cache;

        public AchievementController(
            SoraDbContextFactory factory,
            Cache cache
            )
        {
            _factory = factory;
            _cache = cache;
        }
        
        [HttpGet]
        public IActionResult Index(string achievement)
        {
            byte[] res;
            if ((res = _cache.GetCachedData("jibril:achievements:"+achievement)) != null)
                return File(res, "image/png");
            
            res = Achievements
                .GetAchievement(_factory, achievement.Replace(".png", string.Empty))
                ?.GetIconImage();

            if (res == null)
                return NotFound();
            
            _cache.CacheData("sora:achievements:" + achievement, res, 3600);
            return File(res, "image/png");
        }
    }
}