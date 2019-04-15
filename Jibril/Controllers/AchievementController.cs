using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril.Controllers
{
    [ApiController]
    [Route("/images/medals-client/{achievement}")]
    public class AchievementController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromServices] Cache cache,
                                   [FromServices] Database db,
                                   string achievement)
        {
            byte[] res;
            if ((res = cache.GetCachedData("jibril:achievements:"+achievement)) != null)
                return File(res, "image/png");
            
            res = Achievements
                .GetAchievement(db, achievement.Replace(".png", string.Empty))
                ?.GetIconImage(false);

            if (res == null) return NotFound();
            cache.CacheData("jibril:achievements:" + achievement, res, 3600);
            return File(res, "image/png");
        }
    }
}