using System;
using System.Drawing.Imaging;
using System.IO;
using Devcorner.NIdenticon;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Jibril.Controllers
{
    [ApiController]
    [Route("/{avatarId:int}")]
    public class RootController : Controller
    {
        #region GET /*

        [HttpGet]
        public IActionResult Index([FromServices] Cache cache, int avatarId)
        {
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");

            MemoryStream response = new MemoryStream();
            byte[] x = cache.GetCachedData("jibril:avatars:" + avatarId);
            if (x != null) {
                response.Close();
                return File(x, "image/png");
            }


            if (System.IO.File.Exists("data/avatars/" + avatarId))
            {
                response.Close();
                byte[] file = System.IO.File.ReadAllBytes("data/avatars/" + avatarId);
                cache.CacheData("jibril:avatars:" + avatarId, file, 3600);
                return File(file, "image/png");
            }

            new IdenticonGenerator()
                .Create(Convert.ToString(avatarId, 2))
                .Save(response, ImageFormat.Png);
            response.Position = 0;

            cache.CacheData("jibril:avatars:" + avatarId, response.ToArray(), 3600);
            return File(response, "image/png");
        }
        

        #endregion

    }
}