using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Kaoiji.enums;
using Kaoiji.objects;
using Kaoiji.packets;

namespace Kaoiji.handler
{
    [RegisterHandler(HandlerTypes.Login)]
    public class LoginHandler : IHandler
    {
        public void Run(Presence presence, object data)
        {
            Console.WriteLine("Call");
            PacketWriter pw = new PacketWriter();
            pw.Protocol_Negotiation();
            pw.LoginResponse<LoginResponses>(LoginResponses.FAILED);
            pw.WritePackets(presence.OutputStream);
        }
    }
}
