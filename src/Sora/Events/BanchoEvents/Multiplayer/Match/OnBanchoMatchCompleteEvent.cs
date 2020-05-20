using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchCompleteEvent
    {
        [Event(EventType.BanchoMatchComplete)]
        public void OnBanchoMatchComplete(BanchoMatchCompleteArgs args)
        {
            args.Pr.ActiveMatch?.Complete(args.Pr);
            args.Pr.ActiveMatch?.Update();
        }
    }
}
