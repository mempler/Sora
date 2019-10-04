using Microsoft.AspNetCore.Mvc;

namespace ExamplePlugin
{
    [Route("/")]
    [ApiController]
    public class ExampleController : Controller
    {
        [Route("test")] // we can't override index since it's defined in the main application!
        public string Get() => "response!";
    }
}