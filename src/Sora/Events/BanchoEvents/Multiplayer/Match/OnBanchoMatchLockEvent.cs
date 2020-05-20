using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchLockEvent
    {
        [Event(EventType.BanchoMatchLock)]
        public void OnBanchoMatchLock(BanchoMatchLockArgs args)
        {
            if (args.Pr.ActiveMatch == null ||
                args.Pr.ActiveMatch.HostId != args.Pr.User.Id)
                return;
            
            if (args.SlotId > 16)
                return;

            args.Pr.ActiveMatch.LockSlot(args.Pr.ActiveMatch.Slots[args.SlotId]);
            args.Pr.ActiveMatch.Update();
        }
    }
}
