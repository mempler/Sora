using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sora.Database;
using Sora.Services;

namespace Sora.API.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/status")] // /api/v1/status
    public class ServerStatus : Controller
    {
        private readonly SoraDbContextFactory _factory;
        private readonly Bot.Sora _sora;
        private readonly PresenceService _ps;

        public ServerStatus(SoraDbContextFactory factory, Bot.Sora sora, PresenceService ps)
        {
            _factory = factory;
            _sora = sora;
            _ps = ps;
        }

        private struct ServerStatusResponse
        {
            public int ConnectedUsers;
            public int RegisteredUsers;
        }
        
        [Route("")]
        public async Task<IActionResult> GetIndex()
        {
            var statusResponse = new ServerStatusResponse
            {
                ConnectedUsers = _ps.ConnectedPresences -1, // Sora doesn't count as user!
                RegisteredUsers = await _factory.Get().Users.CountAsync() -1,
            };
            
            return Json(statusResponse);
        }
    }
}
