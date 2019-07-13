using System;
using System.IO;
using System.Net;
using System.Threading;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Sora.Database;
using Sora.Helpers;

namespace Sora
{
    public static class Program
    {
        private static IWebHost _host;
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();

        public enum RecalcOptionTypes
        {
            None,
            Performance,
            Accuracy,
            Duplicates,
            All
        }
        
        public class Options
        {
            [Option('r', "recalc", Required = false, HelpText = "Perform re-calculations")]
            public RecalcOptionTypes RecalcPerformancePoints { get; set; }
        }
        
        private static void Main(string[] args)
        {
            Console.CancelKeyPress += OnProcessExit;
            
            var c = new MemoryCache(new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromDays(365)});
            var cfgUtil = new ConfigUtil(c);

            IServerConfig scfg = cfgUtil.ReadConfig(
                "config.json",
                new Config
                {
                    Server = new CServer
                    {
                        Hostname = "0.0.0.0", Port = 4312, IRCPort = 6667, ScreenshotHostname = "gigamons.de", FreeDirect = true
                    },
                    Cheesegull = new CCheesegull {URI = "https://pisstau.be"},
                    MySql = new CMySql
                    {
                        Hostname = "localhost",
                        Port = 3306,
                        Username = "root",
                        Password = string.Empty,
                        Database = "Sora"
                    }
                }
            );

            _host = new WebHostBuilder()
                    .UseContentRoot(Path.Join(Directory.GetCurrentDirectory(), "/data"))
                    .UseKestrel(
                        cfg =>
                        {
                            cfg.Limits.MaxRequestBodySize = null;

                            if (!IPAddress.TryParse(scfg.Server.Hostname, out var ipAddress))
                                ipAddress = Dns.GetHostEntry(scfg.Server.Hostname).AddressList[0];

                            cfg.Listen(ipAddress, scfg.Server.Port);
                        }
                    )
                    .UseStartup<Startup>()
                    .Build();

            var factory = new SoraDbContextFactory();
            var recalcProcessor = new RecalculationProcessor(factory);
            
            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed(
                      o =>
                      {
                          switch (o.RecalcPerformancePoints)
                          {
                              case RecalcOptionTypes.Performance:
                                  Logger.Info("Performing Recalculation of Performance Points, this will take a long while!");
                                  recalcProcessor.ProcessPerformanceRecalculation();
                                  break;
                              case RecalcOptionTypes.Accuracy:
                                  Logger.Info(
                                      "Performing Recalculation of Accuracy, this will take a long while!"
                                  );
                                  recalcProcessor.ProcessAccuracyRecalculation();
                                  break;
                              case RecalcOptionTypes.Duplicates:
                                  Logger.Info(
                                      "Performing of Deletion of Lower Scores!"
                                  );
                                  recalcProcessor.ProcessLowerScores();
                                  break;
                              case RecalcOptionTypes.All:
                                  Logger.Info(
                                      "Performing Recalculation of Performance Points, this will take a long while!"
                                  );
                                  recalcProcessor.ProcessPerformanceRecalculation();
                                  Logger.Info(
                                      "Performing Recalculation of Accuracy, this will take a long while!"
                                  );
                                  recalcProcessor.ProcessAccuracyRecalculation();
                                  Logger.Info(
                                      "Performing of Deletion of Lower Scores!"
                                  );
                                  recalcProcessor.ProcessLowerScores();
                                  break;
                              case RecalcOptionTypes.None:
                                  break;
                              default:
                                  Logger.Info("RecalcOptionType is not known!");
                                  break;
                          }
                      }
                  );
            
            _host.RunAsync(cts.Token).GetAwaiter().GetResult();
        }

        private static void OnProcessExit(object sender, System.EventArgs e)
        {
            Logger.Info("Killing everything..");
            cts.Cancel();
        }
    }
}
