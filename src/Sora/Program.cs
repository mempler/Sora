using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sora.Framework.Utilities;

namespace Sora
{
    internal static class Program
    {
        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private static IHost host;

        private static async Task Main(string[] args)
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
                    IrcPort = 6667,

                    ScreenShotHostname = "localhost",
                    FreeDirect = true
                },
                Pisstaube = new CPisstaube
                {
                    URI = "https://pisstau.be"
                },
                Esc = Convert.ToBase64String(Crypto.SCrypt.generate_salt())
            };
            
            if (!ConfigUtil.TryReadConfig(out var scfg, "config.json", defaultConfig))
                Environment.Exit(0);
            
            host = Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(builder =>
                            {
                                builder.UseKestrel(cfg =>
                                {
                                    cfg.Limits.MaxRequestBodySize = null;

                                    if (!IPAddress.TryParse(scfg.Server.Hostname, out var ipAddress))
                                        ipAddress = Dns.GetHostEntry(scfg.Server.Hostname).AddressList[0];
                                    cfg.ConfigureEndpointDefaults(cfgEndPoints =>
                                        {
                                            cfgEndPoints.Protocols = HttpProtocols.Http1AndHttp2;
                                        });
                                    
                                    cfg.Listen(IPAddress.Loopback, 5001,
                                        listenOptions => { listenOptions.UseHttps(); });
                                    cfg.Listen(ipAddress, scfg.Server.Port);
                                });

                                builder.UseStartup<StartUp>();
                            })
                            .ConfigureLogging(logging =>
                            {
                                logging.ClearProviders();
                                logging.AddProvider(new SoraLoggerProvider(new SoraLoggerConfiguration
                                {
                                    LogLevel = LogLevel.Trace
                                }));
                            }).Build();

            await host.RunAsync(Cts.Token);
            await host.StopAsync(Cts.Token);
        }

        private static void OnProcessExit(object sender, System.EventArgs e)
        {
            Logger.Info("Killing everything..");
            host?.Dispose(); // This is stupid. using statement ain't working...
            Environment.Exit(0); // we also have to call it manually ...
        }
    }
}