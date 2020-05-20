using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework.Allocation;
using Sora.Framework.Utilities;
using Sora.Services;
using Sora.Utilities;

namespace Sora
{
    public class StartUp
    {
        private readonly IHostingEnvironment _env;
        private Config _config;

        public StartUp(IHostingEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddJsonOptions(jsonOptions => {});
            
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

            _config = scfg;
            
            services.AddSingleton(scfg)
                    .AddSingleton<IConfig>(scfg)
                    .AddSingleton<IMySqlConfig>(scfg)
                    .AddSingleton<IPisstaubeConfig>(scfg)
                    .AddSingleton<IServerConfig>(scfg)
                    .AddSingleton<Pisstaube>()
                    .AddDbContextPool<SoraDbContext>(builder =>
                    {
                        if (_config.MySql.Hostname == null)
                            Logger.Fatal("MySQL Hostname cannot be null!");
                        
                        builder.UseMySql(
                            $"Server={_config.MySql.Hostname};" +
                            $"Database={_config.MySql.Database};" +
                            $"User={_config.MySql.Username};" +
                            $"Password={_config.MySql.Password};" +
                            $"Port={_config.MySql.Port};CharSet=utf8mb4;",
                            mysqlOptions =>
                            {
                                mysqlOptions.CommandTimeout(int.MaxValue);

                                mysqlOptions.ServerVersion(new Version(10, 2, 15), ServerType.MariaDb);
                                mysqlOptions.CharSet(CharSet.Utf8Mb4);
                            }
                        );
                    }, 1024)
                    .AddSingleton<PresenceService>()
                    .AddSingleton<Cache>()
                    .AddSingleton<ChannelService>()
                    .AddSingleton<ConsoleCommandService>()
                    .AddSingleton<Bot.Sora>()
                    .AddSingleton<IrcServer>()
                    .AddSingleton<ChatFilter>()
                    .AddSingleton(new EventManager(new List<Assembly> {Assembly.GetEntryAssembly()}))
                    .AddSingleton<PluginService>();

            services.AddLogging();

            services.AddDbContext<SoraDbContext>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var factory = context.HttpContext.RequestServices.GetRequiredService<SoraDbContext>();
                        if (await DbUser.GetDbUser(factory, context.Principal.Identity.Name) == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        
                        context.Success();
                    }
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    TokenDecryptionKey = new SymmetricSecurityKey(Convert.FromBase64String(scfg.Esc)),
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(scfg.Esc)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            
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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                               .AllowAnyMethod()
                               .AllowCredentials()
                               .SetIsOriginAllowed((host) => true)
                               .AllowAnyHeader());
            });

            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider provider, ILogger<StartUp> logger,
            SoraDbContext dbContext,
            EventManager ev,
            ConsoleCommandService css,
            Bot.Sora sora,
            PresenceService ps,
            PluginService plugs)
        {
            logger.Log(LogLevel.Information, License.l);
            app.UseCors("CorsPolicy");
            
            app.UseMiddleware<ExceptionMiddleware>();
            if (_env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            app.UseAuthentication();
            app.UseWebSockets();

            dbContext.Migrate();

            ev.SetProvider(app);

            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");
            
            if (!Directory.Exists("plugins/runtime"))
                Directory.CreateDirectory("plugins/runtime");
            
            app.UseRouting();
            
            foreach (var dep in Directory.GetFiles("plugins/runtime")) // Dependencies have a higher priority!
                plugs.LoadPlugin(null, Directory.GetCurrentDirectory() + "/" + dep, true);
            
            foreach (var plug in Directory.GetFiles("plugins"))
                plugs.LoadPlugin(app, Directory.GetCurrentDirectory() + "/" + plug);

            ev.RegisterEvents();
            
            css.Start();
            sora.RunAsync();
            
            ps.BeginTimeoutDetector();

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