using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Mailgun;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sora.Framework.Utilities;

namespace Sora.API
{
    public class PluginEntry : Plugin
    {
        private readonly ILogger<PluginEntry> _logger;

        private class EMailConfig : IConfig
        {
            public bool Enabled { get; set; }
            public string APIKey { get; set; }
            public string APIDomain { get; set; }
            public string MailGunRegion { get; set; }
        }

        internal class RecaptchaConfig : IConfig
        {
            public bool Enabled { get; set; }
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
        }

        internal static MailgunSender _sender;
        internal static RecaptchaConfig _recaptchaConfig;
        
        public PluginEntry(ILogger<PluginEntry> logger)
        {
            _logger = logger;
            ConfigUtil.TryReadConfig(out var ec, "mailgun.json", new EMailConfig
            {
                Enabled = false,
                APIKey = string.Empty,
                APIDomain = string.Empty
            });

            if (ec.Enabled)
                _sender = new MailgunSender(
                    ec.APIDomain,
                    ec.APIKey,
                    Enum.Parse<MailGunRegion>(ec.MailGunRegion)
                );

            ConfigUtil.TryReadConfig(out _recaptchaConfig, "recaptcha.json", new RecaptchaConfig
            {
                Enabled = false,
                PublicKey = string.Empty,
                PrivateKey = string.Empty
            });

            Email.DefaultSender = _sender;
        }
        
        public override void OnEnable(IApplicationBuilder app)
        {
            _logger.LogInformation("Startup Sora Web API");

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/api/v1/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await HandleSocket(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });
        }

        public override void OnDisable()
        {
            _logger.LogInformation("Shutdown Sora Web API");
        }
        
        private async Task HandleSocket(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                    result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
