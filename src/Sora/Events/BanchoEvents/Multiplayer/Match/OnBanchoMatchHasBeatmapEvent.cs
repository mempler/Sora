using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchHasBeatmapEvent
    {
        [Event(EventType.BanchoMatchHasBeatmap)]
        public void OnBanchoMatchHasBeatmap(BanchoMatchHasBeatmapArgs args)
        {
            if (args.Pr.ActiveMatch == null)
                return;
            
            var slot = args.Pr.ActiveMatch.GetSlotByUserId(args.Pr.User.Id);

            slot.Status = MultiSlotStatus.NotReady;

            args.Pr.ActiveMatch.Update();
        }
    }
}
