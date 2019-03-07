using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchChangeSlotEvent
    {   
        [Event(EventType.BanchoMatchChangeSlot)]
        public void OnBanchoMatchChangeSlot(BanchoMatchChangeSlotArgs args)
        {
            if (args.pr.JoinedRoom == null) return;
            if (args.SlotId > 16) return;
            MultiplayerSlot newSlot = args.pr.JoinedRoom.Slots[args.SlotId];
            if (newSlot.UserId != -1) return;

            MultiplayerSlot oldSlot = args.pr.JoinedRoom.GetSlotByUserId(args.pr.User.Id);

            args.pr.JoinedRoom.SetSlot(newSlot, oldSlot);
            args.pr.JoinedRoom.ClearSlot(oldSlot);
        }
    }
}