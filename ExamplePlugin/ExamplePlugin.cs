using Kaoiji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaoiji.server;
using System.Reflection;

namespace ExamplePlugin
{
    public class ExamplePlugin : Plugin
    {
        public override void OnStart()
        {
            HttpServer.IndexPage =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("ExamplePlugin.Resources.index.html");

            Console.WriteLine("ExamplePlugin Enabled!");
        }

        public override void OnStop()
        {
            Console.WriteLine("ExamplePlugin Disabled!");
        }
    }
}
