using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Sora
{
    public static class Program
    {
        private static void Main()
        {
            IWebHost host = new WebHostBuilder()
                       .UseContentRoot(Directory.GetCurrentDirectory())
                       .UseKestrel(cfg =>
                       {
                           cfg.ListenAnyIP(4312);
                       })
                       .UseStartup<Startup>()
                       .Build();

            host.Run();
        }
    }
}