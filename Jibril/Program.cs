using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jibril.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Helpers;

namespace Jibril
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseHttpSys(options =>
                   {
                       options.MaxConnections                = 100;
                       options.MaxRequestBodySize            = 30000000;
                       options.UrlPrefixes.Add("http://+:" + ConfigUtil.ReadConfig<Config>().Server.Port);
                   })
                   .UseUrls()
                   .UseStartup<Startup>();
    }
}