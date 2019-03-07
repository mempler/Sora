using EventManager.Attributes;
using EventManager.Enums;
using Shared.Helpers;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;

namespace ExamplePlugin
{
    [EventClass]
    public class ExampleEvent
    {
        // Let the Handler loader know that this is an LoginHandler
        [Event(EventType.BanchoLoginRequest)]
        public void SomeLoginHandler(BanchoLoginRequestArgs args)
        {
            args.Writer.Write(new LoginResponse((LoginResponses) 101));
            args.Writer.Write(new Announce("Hello World!"));
        }
    }
}