using System.IO;
using Jdenticon;
using Microsoft.AspNetCore.Mvc;
using Color = Jdenticon.Rendering.Color;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/{avatarId:int}")]
    public class RootController : Controller
    {
        #region GET /{avatarId}

        [HttpGet]
        public IActionResult Index([FromServices] IServerConfig cfg, int avatarId)
        {
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");

            var response = new MemoryStream();

            if (System.IO.File.Exists("data/avatars/" + avatarId))
            {
                response.Close();
                var file = System.IO.File.ReadAllBytes("data/avatars/" + avatarId);
                return File(file, "image/png");
            }

            var icon = Identicon.FromValue(
                $"{avatarId}{cfg.Server.Hostname}{cfg.Server.Port}",
                1024,
                "SHA384"
            );

            icon.Style.BackColor = Color.Transparent;

            icon.SaveAsPng(response);

            response.Position = 0;

            System.IO.File.WriteAllBytes("data/avatars/" + avatarId, response.GetBuffer());

            return File(response, "image/png");
        }

        #endregion
    }
}
