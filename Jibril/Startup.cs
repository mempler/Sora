using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EventManager.Services;
using Jibril.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;

namespace Jibril
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(ConfigUtil.ReadConfig<Config>())
                .AddSingleton((IConfig) ConfigUtil.ReadConfig<Config>())
                .AddSingleton<Database>()
                .AddSingleton<Cache>();
                
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            Database db,            
            IApplicationBuilder app, IHostingEnvironment env)
        {
            // Create Sora (bot) if not exists.
            if (Users.GetUser(db, 100) == null)
                Users.InsertUser(db, new Users
                {
                    Id         = 100,
                    Username   = "Sora",
                    Email      = "bot@gigamons.de",
                    Password   = "",
                    Privileges = 0
                });

            Localisation.Initialize();
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}