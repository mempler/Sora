using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.Spectator
{
    [EventClass]
    public class OnUserCantSpectateEvent
    {
        [Event(EventType.BanchoCantSpectate)]
        public void OnUserCantSpectate(BanchoCantSpectateArgs args)
        {
            args.Pr.Spectator?.Push(new SpectatorCantSpectate(args.Pr.User.Id));
        }
    }
}
