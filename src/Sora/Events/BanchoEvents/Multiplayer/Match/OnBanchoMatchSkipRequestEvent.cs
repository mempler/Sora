using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchSkipRequestEvent
    {
        [Event(EventType.BanchoMatchSkipRequest)]
        public void OnBanchoMatchSkipRequest(BanchoMatchSkipRequestArgs args)
        {
            args.Pr.ActiveMatch?.Skip(args.Pr);
        }
    }
}
