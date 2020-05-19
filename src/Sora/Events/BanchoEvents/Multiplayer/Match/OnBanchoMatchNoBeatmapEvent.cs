using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Enums;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchNoBeatmapEvent
    {
        [Event(EventType.BanchoMatchNoBeatmap)]
        public void OnBanchoMatchNoBeatmap(BanchoMatchNoBeatmapArgs args)
        {
            if (args.Pr.ActiveMatch == null)
                return;
            
            var slot = args.Pr.ActiveMatch.GetSlotByUserId(args.Pr.User.Id);

            slot.Status = MultiSlotStatus.NoMap;

            args.Pr.ActiveMatch.Update();
        }
    }
}
