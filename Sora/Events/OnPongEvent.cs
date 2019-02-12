using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;

namespace Sora.Events
{
    [EventClass]
    public class OnPongEvent
    {
        [Event(EventType.BanchoPong)]
        public void OnPong(BanchoPongArgs args)
        {
            if (args.pr.Spectator == null || args.pr.Spectator?.BoundPresence == args.pr) return;
            
            args.pr.Spectator?.Broadcast(new UserPresence(args.pr));
            args.pr.Spectator?.Broadcast(new HandleUpdate(args.pr));
        }
    }
}