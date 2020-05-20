using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchReadyEvent
    {
        [Event(EventType.BanchoMatchReady)]
        public void OnBanchoMatchReady(BanchoMatchReadyArgs args)
        {
            var slot = args.Pr.ActiveMatch?.GetSlotByUserId(args.Pr.User.Id);
            if (slot == null)
                return;

            slot.Status = MultiSlotStatus.Ready;

            args.Pr.ActiveMatch?.Update();
        }
    }
}
