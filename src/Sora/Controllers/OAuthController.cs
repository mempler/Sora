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
        private readonly SoraDbContext _ctx;
        private readonly Config _cfg;
        private readonly ILogger<OAuthController> _logger;

        public interface IOAuth
        {
            string GrantType { get; set; }
            string ClientId { get; set; }
            string ClientSecret { get; set; }
        }

        public struct PasswordAuth
        {
            public string Username;
            public string Password;
        }
        
        public struct OAuthPasswordAuth : IOAuth
        {
            public string Username;
            public string Password;
            public string GrantType { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            
            public override string ToString()
                => $"U: {Username} PW: {Password} GT: {GrantType} CID: {ClientId} CS: {ClientSecret}";
        }

        public OAuthController(SoraDbContext ctx, Config cfg, ILogger<OAuthController> logger)
        {
            _ctx = ctx;
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
            
            var user = await DbUser.GetDbUser(_ctx, auth.Username);
            if (user == null)
                return BadRequest(new {code = 401, message = "Username is incorrect!"});
            if (!user.IsPassword(Hex.ToHex(Crypto.GetMd5(auth.Password))))
                return BadRequest(new {code = 402, message = "Password is incorrect"});

            var key = Convert.FromBase64String(_cfg.Esc);
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

            var oauthClient = await DboAuthClient.GetClient(_ctx, auth.ClientId);
            if (oauthClient == null)
                return BadRequest(new {code = 403, message = "Client Doesn't exists!"});
            if (oauthClient.Secret != auth.ClientSecret)
                return BadRequest(new {code = 404, message = "invalid secret!"});
            
            var user = await DbUser.GetDbUser(_ctx, auth.Username);
            if (user == null || !user.IsPassword(Hex.ToHex(Crypto.GetMd5(auth.Password))))
                return BadRequest(new {code = 400, message = "Username or password is incorrect"});

            var key = Convert.FromBase64String(_cfg.Esc);
            return Authorized(key, new[] {new Claim(ClaimTypes.Name, user.Id.ToString())});
        }
    }
}