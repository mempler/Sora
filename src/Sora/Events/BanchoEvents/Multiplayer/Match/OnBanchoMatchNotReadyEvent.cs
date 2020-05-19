using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchNotReadyEvent
    {
        [Event(EventType.BanchoMatchNotReady)]
        public void OnBanchoMatchNotReady(BanchoMatchNotReadyArgs args)
        {
            var slot = args.Pr.ActiveMatch?.GetSlotByUserId(args.Pr.User.Id);
            if (slot == null)
                return;

            slot.Status = MultiSlotStatus.NotReady;

            args.Pr.ActiveMatch.Update();
        }
    }
}
