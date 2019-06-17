using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
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
            
            _host = new WebHostBuilder()
                       .UseContentRoot(Directory.GetCurrentDirectory())
                       .UseKestrel(cfg =>
                       {
                           cfg.Limits.MaxRequestBodySize = null;
                           cfg.ListenAnyIP(ConfigUtil.ReadConfig<Config>().Server.Port);
                       })
                       .UseStartup<Startup>()
                       //.UseShutdownTimeout(TimeSpan.FromSeconds(10))
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