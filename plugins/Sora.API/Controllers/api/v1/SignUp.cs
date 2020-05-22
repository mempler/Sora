using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Colorful;
using FluentEmail.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework;
using Sora.Framework.Utilities;

namespace Sora.API.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/signup")] // /api/v1/signup
    public class SignUp : Controller
    {
        private SoraDbContext _context;
        private readonly IServerConfig _serverConfig;

        public SignUp(SoraDbContext context, IServerConfig serverConfig)
        {
            _context = context;
            _serverConfig = serverConfig;
        }

        private struct SignUpRequest
        {
            public string EMail { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string GCaptchaValidation { get; set; }
        }

        private struct GCaptchaVerificationResponse
        {
            public bool success { get; set; }
        }
        
        [HttpPost]
        [DisableCors]
        public async Task<ActionResult> Post()
        {
            SignUpRequest credentials;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                credentials = JsonConvert.DeserializeObject<SignUpRequest>(await reader.ReadToEndAsync());
            }

            if (PluginEntry._recaptchaConfig.Enabled) {
                using var wc = new WebRequest("https://www.google.com/recaptcha/api/siteverify") {Method = HttpMethod.Post};
                wc.AddParameter("secret", PluginEntry._recaptchaConfig.PrivateKey);
                wc.AddParameter("response", credentials.GCaptchaValidation);
                await wc.PerformAsync();

                var gCaptcha = JsonConvert.DeserializeObject<GCaptchaVerificationResponse>(wc.ResponseString);
                if (!gCaptcha.success)
                    return Ok(new
                    {
                        code = 450,
                        message = "Recaptcha failed!"
                    });
            }
            
            var u = await DbUser.RegisterUser(_context, Permission.From(Permission.DEFAULT), credentials.UserName,
                credentials.EMail,
                credentials.Password, false);

            if (PluginEntry._sender == null)
                return Ok(new
                {
                    code = 0,
                    message = "Success"
                });
            
            var emailKey = Crypto.RandomString(512);
            u.Status = UserStatusFlags.Suspended;
            u.StatusReason = "Verification|" + emailKey;
            u.StatusUntil =
                DateTime.Today + TimeSpan.FromDays(365 * 100); // Suspend for 100 years (or until EMail Verified!)
            _context.Update(u);

            var em = Email
                     .From("no-reply@gigamons.de")
                     .To(u.EMail)
                     .Subject("Email Confirmation")
                     .Body("Your EMail has to be Verificated! Please click <a href=\"" +
                           "http://" + _serverConfig.Server.ScreenShotHostname + "/verificate?k" + emailKey
                           + "\">Here</a>!");

            var r = await em.SendAsync();
            if (!r.Successful)
                throw new Exception(
                    $"Failed to send EMail: {r.Successful} {r.MessageId} {r.ErrorMessages.Aggregate((a, b) => a + ", " + b)}"
                );

            return Ok(new
            {
                code = 100,
                message = "EMail Verification Pending"
            });
        }
    }
}