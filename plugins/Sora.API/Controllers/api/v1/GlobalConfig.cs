using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Sora.API.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/config")] // /api/v1/config
    public class GlobalConfig : Controller
    {
        [HttpGet]
        [DisableCors]
        public ActionResult Get() => Json(new
        {
            GoogleRecaptchaKey = PluginEntry._recaptchaConfig.PublicKey
        });
    }
}