using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sora.Database;
using Sora.Database.Models;

namespace Sora.API.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/latest_donators")] // /api/v1/latest_donators
    public class LatestDonators : Controller
    {
        private SoraDbContextFactory _factory;

        public LatestDonators(SoraDbContextFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [DisableCors]
        public async Task<ActionResult> Get()
        {
            var users = await _factory.Get().Users.Where(u => u.Status == UserStatusFlags.Donator &&
                                                              u.StatusUntil >= DateTime.Now)
                                .OrderByDescending(x => x.StatusUntil)
                                .Take(20)
                                .Select(x =>
                                    new
                                    {
                                        x.Id,
                                        x.UserName,
                                        Until = x.StatusUntil
                                    }).ToListAsync();
            return Ok(JsonConvert.SerializeObject(users));
        }
    }
}