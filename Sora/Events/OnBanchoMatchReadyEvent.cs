using EventManager.Attributes;
using EventManager.Enums;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchReadyEvent
    {
        [Event(EventType.BanchoMatchReady)]
        public void OnBanchoMatchReady(BanchoMatchReadyArgs args)
        {
            MultiplayerSlot slot = args.pr.JoinedRoom?.GetSlotByUserId(args.pr.User.Id);
            if (slot == null) return;

            slot.Status = MultiSlotStatus.Ready;

            args.pr.JoinedRoom.Update();
        }
    }
}