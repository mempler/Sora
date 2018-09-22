using Shared.Enums;
using Sora.Handler;
using Sora.Packets.Server;
using Sora.Server;

namespace ExamplePlugin
{
    internal class ExampleHandler
    {
        // Let the Handler loader know that this is an LoginHandler
        [Handler(HandlerTypes.LoginHandler)]
        public void SomeLoginHandler(Req req, Res res) // a Login handler need those args.
        {
            // Write an Announce Packet to client.
            res.Writer.Write(new Announce("Hello World!"));

            // Done, you've successfully send an Hello World Packet!
        }
    }
}
