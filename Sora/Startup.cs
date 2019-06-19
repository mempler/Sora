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
using Sora.Database;
using Sora.Enums;
using Sora.Helpers;
using Sora.Services;
using Logger = Sora.Helpers.Logger;
using Users = Sora.Database.Models.Users;

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

            MemoryCache c = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromDays(365)
            });
            ConfigUtil cfgUtil = new ConfigUtil(c);

            Config scfg = cfgUtil.ReadConfig<Config>();
                
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
                    .AddSingleton<PerformancePointsProcessor>()

                    .AddSingleton(new EventManager(new List<Assembly> {Assembly.GetEntryAssembly()}));

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit             = int.MaxValue;
                x.MultipartBodyLengthLimit     = int.MaxValue;
                x.MemoryBufferThreshold        = int.MaxValue;
                x.BufferBodyLengthLimit        = int.MaxValue;
                x.MultipartBoundaryLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit  = int.MaxValue;
            });
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
                              Bot.Sora s
            )
        {
            app.UseMiddleware<LoggingMiddleware>();
            
            Logger.Info(@"%#FFFFFF%Sora V1.0.0

%#800000%=============================== %#F94848%License %#800000%=================================
%#F94848%Sora - A Modular Bancho written in C#
Copyright (C) 2019 Robin A. P.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
%#800000%==========================================================================

");

            Stopwatch w = new Stopwatch();
            Logger.Info("Generating %#F94848%Database%#FFFFFF%! this could take a while.");
            w.Start();
            // Create Sora (bot) if not exists.
            if (Users.GetUser(factory, 100) == null)
                Users.InsertUser(factory, new Users
                {
                    Id         = 100,
                    Username   = "Sora",
                    Email      = "bot@gigamons.de",
                    Password   = "",
                    Privileges = Privileges.Admin
                });
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

            foreach (string plug in Directory.GetFiles("plugins"))
                plugs.LoadPlugin(Directory.GetCurrentDirectory() + "/" + plug);
            
            ev.RegisterEvents();
            
            ps.TimeoutCheck();

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            app.UseDeveloperExceptionPage();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public class LoggingMiddleware
        {
            private readonly RequestDelegate _next;

            public LoggingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.Err(ex);
                    await _next(context);
                }
            }
        }
    }
}