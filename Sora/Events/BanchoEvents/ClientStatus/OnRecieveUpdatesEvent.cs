using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnRecieveUpdatesEvent
    {
        [Event(EventType.BanchoReceiveUpdates)]
        public void OnRecieveUpdates(BanchoEmptyEventArgs args)
        {
            args.pr += new UserPresence(args.pr);
            args.pr += new HandleUpdate(args.pr);
        }
    }
}