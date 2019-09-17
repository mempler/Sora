using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sora.Database;
using Sora.Framework.Allocation;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora
{
    public class StartUp
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration config;

        public StartUp(IHostingEnvironment env)
        {
            _env = env;
            
            config = new ConfigurationBuilder().Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
            services.AddMemoryCache();
            
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
            
            if (!ConfigUtil.TryReadConfig(out var scfg, "config.json", defaultConfig))
                Environment.Exit(0);
            
            services.AddSingleton(scfg)
                    .AddSingleton<IConfig>(scfg)
                    .AddSingleton<IMySQLConfig>(scfg)
                    .AddSingleton<IPisstaubeConfig>(scfg)
                    .AddSingleton<IServerConfig>(scfg)
                    .AddSingleton<Pisstaube>()
                    .AddSingleton<SoraDbContextFactory>()
                    .AddSingleton<PluginService>()
                    .AddSingleton<PresenceService>()
                    .AddSingleton<Cache>()
                    .AddSingleton<ChannelService>()
                    .AddSingleton<Bot.Sora>()
                    .AddSingleton<IRCServer>()
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

        public void Configure(IApplicationBuilder app, IServiceProvider provider,
            SoraDbContextFactory factory,
            EventManager ev,
            PluginService plugs)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            if (_env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            factory.Get().Migrate();

            ev.SetProvider(provider);

            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");

            foreach (var plug in Directory.GetFiles("plugins"))
                plugs.LoadPlugin(Directory.GetCurrentDirectory() + "/" + plug);

            ev.RegisterEvents();

            
            app.UseMvc(
                routes => routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}"
                )
            );
        }
    }
    
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                throw;
            }
        }
    }
}