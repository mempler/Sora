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
    class ExampleHandler : BaseHandler
    {
        public override void Run(Presence presence, object data, HttpListenerResponse writer)
        {
            PacketList pl = new PacketList();
            Console.WriteLine("[ExamplePlugin] Token: {0}", presence);
            pl.Append(new Announce($"Your Token: {presence.Token}"));
            writer.ContentLength64 += pl.ToBinary().Length;
            writer.OutputStream.Write(pl.ToBinary(), (int) writer.OutputStream.Position, pl.ToBinary().Length);
        }
    }
}
