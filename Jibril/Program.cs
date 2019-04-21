using Jibril.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
                   .UseKestrel(options =>
                   {
                       options.Limits.MaxRequestBodySize = 52428800; //50MB
                   })
                   .UseUrls("http://+:" + ConfigUtil.ReadConfig<Config>().Server.Port)
                   .UseStartup<Startup>();
    }
}