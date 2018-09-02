using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Kaoiji.enums;
using Kaoiji.objects;

namespace Kaoiji.handler
{
    [RegisterHandler(HandlerTypes.Login)]
    public class LoginHandler : BaseHandler
    {
        public override void Run(Presence presence, object data, HttpListenerResponse writer)
        {
            Console.WriteLine(data);
        }
    }
}
