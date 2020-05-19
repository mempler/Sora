using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchChangeSlotEvent
    {
        [Event(EventType.BanchoMatchChangeSlot)]
        public void OnBanchoMatchChangeSlot(BanchoMatchChangeSlotArgs args)
        {
            var match = args.Pr.ActiveMatch;
            if (match == null ||
                args.SlotId > 16)
                return;
            
            var newSlot = match.Slots[args.SlotId];
            if (newSlot.UserId != -1)
                return;

            var oldSlot = match.GetSlotByUserId(args.Pr.User.Id);

            match.SetSlot(newSlot, oldSlot);
            match.ClearSlot(oldSlot);
            match.Update();
        }
    }
}
