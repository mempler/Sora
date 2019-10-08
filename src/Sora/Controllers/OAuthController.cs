using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework.Utilities;

namespace Sora.Controllers
{
    [Authorize]
    [Route("/oauth")]
    [ApiController]
    public class OAuthController : Controller
    {
        private readonly SoraDbContextFactory _factory;
        private readonly Config _cfg;
        private readonly ILogger<OAuthController> _logger;

        public interface IOAuth
        {
            string grant_type { get; set; }
            string client_id { get; set; }
            string client_secret { get; set; }
        }

        public struct PasswordAuth
        {
            public string username;
            public string password;
        }
        
        public struct OAuthPasswordAuth : IOAuth
        {
            public string username;
            public string password;
            public string grant_type { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            
            public override string ToString()
                => $"U: {username} PW: {password} GT: {grant_type} CID: {client_id} CS: {client_secret}";
        }

        public OAuthController(SoraDbContextFactory factory, Config cfg, ILogger<OAuthController> logger)
        {
            _factory = factory;
            _cfg = cfg;
            _logger = logger;
        }

        private IActionResult Authorized(byte[] key, IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = "everyone",
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(key),
                    JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token_type = "Bearer",
                expires_in = Math.Round((double) tokenDescriptor.Expires?.Subtract(DateTime.UtcNow).TotalSeconds),
                access_token = tokenHandler.WriteToken(token)
            });
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> AuthWebsite()
        {
            PasswordAuth auth;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                auth = JsonConvert.DeserializeObject<PasswordAuth>(await reader.ReadToEndAsync());
            }
            
            var user = await DBUser.GetDBUser(_factory, auth.username);
            if (user == null)
                return BadRequest(new {code = 401, message = "Username is incorrect!"});
            if (!user.IsPassword(Hex.ToHex(Crypto.GetMd5(auth.password))))
                return BadRequest(new {code = 402, message = "Password is incorrect"});

            var key = Convert.FromBase64String(_cfg.ESC);
            return Authorized(key, new [] { new Claim(ClaimTypes.Name, user.Id.ToString()), });
        }
        
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> AuthPassword()
        {
            OAuthPasswordAuth auth;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                auth = JsonConvert.DeserializeObject<OAuthPasswordAuth>(await reader.ReadToEndAsync());
            }

            var oauthClient = await DBOAuthClient.GetClient(_factory, auth.client_id);
            if (oauthClient == null)
                return BadRequest(new {code = 403, message = "Client Doesn't exists!"});
            if (oauthClient.Secret != auth.client_secret)
                return BadRequest(new {code = 404, message = "invalid secret!"});
            
            var user = await DBUser.GetDBUser(_factory, auth.username);
            if (user == null || !user.IsPassword(Hex.ToHex(Crypto.GetMd5(auth.password))))
                return BadRequest(new {code = 400, message = "Username or password is incorrect"});

            var key = Convert.FromBase64String(_cfg.ESC);
            return Authorized(key, new[] {new Claim(ClaimTypes.Name, user.Id.ToString())});
        }
    }
}