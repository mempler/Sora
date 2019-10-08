using System;
using System.Linq;
using FluentEmail.Core;
using FluentEmail.Mailgun;
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
        
        public override void OnEnable()
        {
            _logger.LogInformation("Startup Sora Web API");
        }

        public override void OnDisable()
        {
            _logger.LogInformation("Shutdown olSora Web API");
        }
    }
}
