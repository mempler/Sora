using System;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Sora.Helpers;

namespace Sora
{
    public static class Program
    {
        private static IWebHost _host;
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();
        private static void Main()
        {
            Console.CancelKeyPress += OnProcessExit;

            MemoryCache c = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromDays(365)
            });
            ConfigUtil cfgUtil = new ConfigUtil(c);

            IServerConfig scfg = cfgUtil.ReadConfig("config.json", new Config
            {
                Server = new CServer
                {
                    Hostname   = "0.0.0.0",
                    Port       = 4312,
                    FreeDirect = true
                },
                Cheesegull = new CCheesegull
                {
                    URI = "https://pisstau.be"
                },
                MySql = new CMySql
                {
                    Hostname = "localhost",
                    Port     = 3306,
                    Username = "root",
                    Password = string.Empty,
                    Database = "Sora"
                }
            });
            
            _host = new WebHostBuilder()
                       .UseContentRoot(Directory.GetCurrentDirectory())
                       .UseKestrel(cfg =>
                       {
                           cfg.Limits.MaxRequestBodySize = null;

                           if (!IPAddress.TryParse(scfg.Server.Hostname, out IPAddress ipAddress))
                               ipAddress = Dns.GetHostEntry(scfg.Server.Hostname).AddressList[0];
                           
                           cfg.Listen(ipAddress, scfg.Server.Port);
                       })
                       .UseStartup<Startup>()
                       .Build();
            _host.RunAsync(cts.Token).GetAwaiter().GetResult();
        }

        private static void OnProcessExit(object sender, System.EventArgs e)
        {
            Logger.Info("Killing everything..");
            cts.Cancel();
        }
    }
}