using Kaoiji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplePlugin
{
    public class ExamplePlugin : Plugin
    {
        public override void OnStart()
        {
            Console.WriteLine("ExamplePlugin Enabled!");
        }

        public override void OnStop()
        {
            Console.WriteLine("ExamplePlugin Disabled!");
        }
    }
}
