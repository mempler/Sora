using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                    .AddSingleton<ConsoleCommandService>()
                    .AddSingleton<Bot.Sora>()
                    .AddSingleton<IRCServer>()
                    .AddSingleton(new EventManager(new List<Assembly> {Assembly.GetEntryAssembly()}));

            services.AddLogging();
            
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

        public void Configure(IApplicationBuilder app, IServiceProvider provider, ILogger<StartUp> logger,
            SoraDbContextFactory factory,
            EventManager ev,
            ConsoleCommandService css,
            Bot.Sora sora,
            PluginService plugs)
        {
            logger.Log(LogLevel.Information, License.L);

            app.UseMiddleware<ExceptionMiddleware>();
            if (_env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            factory.Get().Migrate();

            ev.SetProvider(provider);

            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");

            foreach (var plug in Directory.GetFiles("plugins"))
                plugs.LoadPlugin(Directory.GetCurrentDirectory() + "/" + plug);

            ev.RegisterEvents();
            
            css.Start();
            sora.RunAsync();

            app.UseMvc(
                routes => routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}"
                )
            );
        }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory?.CreateLogger<ExceptionMiddleware>() ??
                      throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                    throw;

                _logger.LogError("{ex}", ex);

                context.Response.Clear();
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Server side Error! Please Report\n");
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}