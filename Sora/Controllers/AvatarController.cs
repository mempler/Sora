using System;
using System.Drawing.Imaging;
using System.IO;
using Devcorner.NIdenticon;
using Microsoft.AspNetCore.Mvc;
using Sora.Helpers;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/{avatarId:int}")]
    public class RootController : Controller
    {
        #region GET /{avatarId}

        [HttpGet]
        public IActionResult Index([FromServices] Cache cache, int avatarId)
        {
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");
            
            MemoryStream response = new MemoryStream();
            
            if (System.IO.File.Exists("data/avatars/" + avatarId))
            {
                response.Close();
                byte[] file = System.IO.File.ReadAllBytes("data/avatars/" + avatarId);
                return File(file, "image/png");
            }
            
            new IdenticonGenerator()
                .Create(Convert.ToString(avatarId, 2))
                .Save(response, ImageFormat.Png);
            response.Position = 0;
            
            System.IO.File.WriteAllBytes("data/avatars/" + avatarId, response.GetBuffer());
            
            return File(response, "image/png");
        }

        #endregion
    }
}