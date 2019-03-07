using EventManager.Attributes;
using EventManager.Enums;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchHasBeatmapEvent
    {
        [Event(EventType.BanchoMatchHasBeatmap)]
        public void OnBanchoMatchHasBeatmap(BanchoMatchHasBeatmapArgs args)
        {
            if (args.pr.JoinedRoom == null)
                return;
            MultiplayerSlot slot = args.pr.JoinedRoom.GetSlotByUserId(args.pr.User.Id);

            slot.Status = MultiSlotStatus.NotReady;

            args.pr.JoinedRoom.Update();
        }
    }
}