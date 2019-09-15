using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sora.Allocation;
using Sora.Database;
using Sora.Database.Models;
using Sora.Helpers;
using Sora.Server;
using Sora.Services;

namespace Sora
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            //.SetBasePath(env.ContentRootPath);

            config = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
            services.AddMemoryCache();

            var c = new MemoryCache(new MemoryCacheOptions {ExpirationScanFrequency = TimeSpan.FromDays(365)});
            var cfgUtil = new ConfigUtil(c);

            var scfg = cfgUtil.ReadConfig<Config>();

            var f = new SoraDbContextFactory();
            f.Get().Migrate();

            services.AddSingleton(scfg)
                    .AddSingleton<IConfig>(scfg)
                    .AddSingleton<IMySQLConfig>(scfg)
                    .AddSingleton<ICheesegullConfig>(scfg)
                    .AddSingleton<IServerConfig>(scfg)
                    .AddSingleton<ConfigUtil>()
                    .AddSingleton<SoraDbContextFactory>()
                    .AddSingleton<PluginService>()
                    .AddSingleton<PresenceService>()
                    .AddSingleton<MultiplayerService>()
                    .AddSingleton<PacketStreamService>()
                    .AddSingleton<Cache>()
                    .AddSingleton<ConsoleCommandService>()
                    .AddSingleton<ChannelService>()
                    .AddSingleton<Bot.Sora>()
                    .AddSingleton<IRCServer>()
                    .AddSingleton<PerformancePointsProcessor>()
                    .AddSingleton(new EventManager(new List<Assembly> {Assembly.GetEntryAssembly()}));

            services.Configure<FormOptions>(
                x =>
                {
                    x.ValueLengthLimit = int.MaxValue;
                    x.MultipartBodyLengthLimit = int.MaxValue;
                    x.MemoryBufferThreshold = int.MaxValue;
                    x.BufferBodyLengthLimit = int.MaxValue;
                    x.MultipartBoundaryLengthLimit = int.MaxValue;
                    x.MultipartHeadersLengthLimit = int.MaxValue;
                }
            );
        }

        public void Configure(IApplicationBuilder app,
            IServiceProvider provider,
            SoraDbContextFactory factory,
            Config config,
            IConfig icfg,
            PluginService plugs,
            EventManager ev,
            IHostingEnvironment env,
            MultiplayerService mps,
            PacketStreamService pss,
            ChannelService cs,
            PresenceService ps,
            ConsoleCommandService ccs,
            Bot.Sora s,
            IRCServer ircServer
        )
        {
            Logger.Info(Constants.License);
            
            var w = new Stopwatch();
            Logger.Info("Generating %#F94848%Database%#FFFFFF%! this could take a while.");
            w.Start();
            // Create olSora (bot) if not exists.
            if (Users.GetUser(factory, 100) == null)
                Users.InsertUser(
                    factory, new Users
                    {
                        Id = 100,
                        Username = "olSora",
                        Email = "bot@gigamons.de",
                        Password = "",
                        Permissions = Permission.From(Permission.GROUP_ADMIN)
                    }
                );
            w.Stop();
            Logger.Info("Done, it took%#3cfc59%", w.ElapsedMilliseconds + "ms");

            AchievementProcessor.CreateDefaultAchievements(factory);
            Localisation.Initialize();

            ev.SetProvider(provider);

            if (Environment.GetEnvironmentVariable("COS_READONLY") == null)
                ccs.Start();
            s.RunAsync();

            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");

            foreach (var plug in Directory.GetFiles("plugins"))
                plugs.LoadPlugin(Directory.GetCurrentDirectory() + "/" + plug);

            ev.RegisterEvents();

            ps.TimeoutCheck();

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            app.UseDeveloperExceptionPage();

            app.UseMvc(
                routes => routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}"
                )
            );

            ircServer.StartAsync();
        }
    }
}
