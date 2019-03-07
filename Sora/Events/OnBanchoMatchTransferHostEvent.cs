using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchTransferHostEvent
    {
        [Event(EventType.BanchoMatchTransferHost)]
        public void OnBanchoMatchTransferHost (BanchoMatchTransferHostArgs args)
        {
            if (args.pr.JoinedRoom == null || args.pr.JoinedRoom.HostId != args.pr.User.Id)
                return;
            if (args.SlotId > 16) return;
            MultiplayerSlot slot = args.pr.JoinedRoom.Slots[args.SlotId];

            args.pr.JoinedRoom.SetHost(slot.UserId);
        }
    }
}
