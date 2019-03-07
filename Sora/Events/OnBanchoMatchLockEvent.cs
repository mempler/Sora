using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchLockEvent
    {
        [Event(EventType.BanchoMatchLock)]
        public void OnBanchoMatchLock(BanchoMatchLockArgs args)
        {
            if (args.pr.JoinedRoom == null || args.pr.JoinedRoom.HostId != args.pr.User.Id) return;
            if (args.SlotId > 16) return;

            args.pr.JoinedRoom.LockSlot(args.pr.JoinedRoom.Slots[args.SlotId]);
        }
    }
}