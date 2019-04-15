using System.IO;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Jibril.Controllers
{
    [ApiController]
    [Route("/ss/{screenshot}")]
    public class ScreenshotController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromServices] Cache cache, string screenshot)
        {
            if (!Directory.Exists("data/screenshots"))
                Directory.CreateDirectory("data/screenshots");

            MemoryStream response = new MemoryStream();
            byte[]       x        = cache.GetCachedData("jibril:screenshots:" + screenshot);
            if (x != null)
            {
                response.Close();
                return File(x, "image/jpg");
            }

            if (!System.IO.File.Exists("data/screenshots/" + screenshot))
                return NotFound($"Could not find Screenshot with the ID of {screenshot}");
            
            
            response.Close();
            byte[] file = System.IO.File.ReadAllBytes("data/screenshots/" + screenshot);
            cache.CacheData("jibril:screenshots:" + screenshot, file, 3600);
            return File(file, "image/jpg");
        }
    }
}