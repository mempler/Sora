using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;

namespace Sora.Events
{
    [EventClass]
    public class OnUserCantSpectateEvent
    {
        [Event(EventType.BanchoCantSpectate)]
        public void OnUserCantSpectate(BanchoCantSpectateArgs args)
        {
            args.pr.Spectator?.Broadcast(new SpectatorCantSpectate(args.pr.User.Id));
        }
    }
}