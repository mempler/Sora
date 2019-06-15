using System.IO;
using Microsoft.AspNetCore.Mvc;
using Sora.Helpers;

namespace Jibril.Controllers
{
    [ApiController]
    [Route("/ss/{screenshot}")]
    public class ScreenshotController : Controller
    {
        private readonly Cache _cache;

        public ScreenshotController(Cache cache)
        {
            _cache = cache;
        }
        
        [HttpGet]
        public IActionResult Index(string screenshot)
        {
            if (!Directory.Exists("data/screenshots"))
                Directory.CreateDirectory("data/screenshots");
            
            byte[] x = _cache.GetCachedData("sora:screenshots:" + screenshot);
            
            if (x != null)
            {
                return File(x, "image/jpg");
            }

            if (!System.IO.File.Exists("data/screenshots/" + screenshot))
                return NotFound($"Could not find Screenshot with the ID of {screenshot}");
            
            byte[] file = System.IO.File.ReadAllBytes("data/screenshots/" + screenshot);
            _cache.CacheData("sora:screenshots:" + screenshot, file, 3600);
            return File(file, "image/jpg");
        }
    }
}