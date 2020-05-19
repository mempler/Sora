using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchFailedEvent
    {
        [Event(EventType.BanchoMatchFailed)]
        public void OnBanchoMatchFailed(BanchoMatchFailedArgs args)
        {
            args.Pr.ActiveMatch?.Failed(args.Pr);
            args.Pr.ActiveMatch?.Update();
        }
    }
}
