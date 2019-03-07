using EventManager.Attributes;
using EventManager.Enums;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchNotReadyEvent
    {
        [Event(EventType.BanchoMatchNotReady)]
        public void OnBanchoMatchNotReady(BanchoMatchNotReadyArgs args)
        {
            MultiplayerSlot slot = args.pr.JoinedRoom?.GetSlotByUserId(args.pr.User.Id);
            if (slot == null) return;

            slot.Status = MultiSlotStatus.NotReady;

            args.pr.JoinedRoom.Update();
        }
    }
}