using System;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Sora.Framework.Utilities;

namespace Sora
{
    internal static class Program
    {
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();
        
        private static void Main()
        {
            Console.CancelKeyPress += OnProcessExit;
            
            var defaultConfig = new Config
            {
                MySql = new CMySql
                {
                    Hostname = "localhost",
                    Port = 3306,
                    Database = "sora",
                    Username = "sora",
                    Password = string.Empty,
                },
                Server = new CServer
                {
                    Hostname = "0.0.0.0",
                    Port = 5001,
                    IRCPort = 6667,

                    ScreenShotHostname = "localhost",
                    FreeDirect = true
                },
                Pisstaube = new CPisstaube
                {
                    URI = "https://pisstau.be"
                }
            };
            
            Logger.Info(License.L);
            if (!ConfigUtil.TryReadConfig(out var scfg, "config.json", defaultConfig))
                Environment.Exit(0);


            var _host = new WebHostBuilder()
                    .UseKestrel(
                        cfg =>
                        {
                            cfg.Limits.MaxRequestBodySize = null;

                            if (!IPAddress.TryParse(scfg.Server.Hostname, out var ipAddress))
                                ipAddress = Dns.GetHostEntry(scfg.Server.Hostname).AddressList[0];

                            cfg.Listen(ipAddress, scfg.Server.Port);
                        }
                    )
                    .UseStartup<StartUp>()
                    .Build();
            
            _host.RunAsync(cts.Token).Wait();
        }

        private static void OnProcessExit(object sender, System.EventArgs e)
        {
            Logger.Info("Killing everything..");
            cts.Cancel();
        }
    }
}