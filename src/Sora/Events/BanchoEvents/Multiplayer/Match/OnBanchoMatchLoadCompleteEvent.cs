using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchLoadCompleteEvent
    {
        [Event(EventType.BanchoMatchLoadComplete)]
        public void OnBanchoMatchLoadComplete(BanchoMatchLoadCompleteArgs args)
        {
            if (args.Pr.ActiveMatch?.GetSlotByUserId(args.Pr.User.Id) != null)
                args.Pr.ActiveMatch.LoadComplete();

            args.Pr.ActiveMatch?.Update();
        }
    }
}
