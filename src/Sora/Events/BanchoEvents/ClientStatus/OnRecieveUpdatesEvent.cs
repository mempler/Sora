using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnRecieveUpdatesEvent
    {
        [Event(EventType.BanchoReceiveUpdates)]
        public void OnRecieveUpdates(BanchoEmptyEventArgs args)
        {
            args.pr.Push(new UserPresence(args.pr));
            args.pr.Push(new HandleUpdate(args.pr));
        }
    }
}
