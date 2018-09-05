using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Kaoiji.enums;
using Kaoiji.handler;
using Kaoiji.objects;
using Kaoiji.packets;

namespace ExamplePlugin
{
    [RegisterHandler(HandlerTypes.Login)]
    class ExampleHandler : IHandler
    {
        public void Run(Presence presence, object data)
        {
            Console.WriteLine("[ExamplePlugin] Token: {0}", presence);
            PacketWriter pw = new PacketWriter();
            pw.Announce($"Your Token: {presence.Token}");
            pw.WritePackets(presence.OutputStream);
        }
    }
}
