using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchScoreUpdateEvent
    {
        [Event(EventType.BanchoMatchScoreUpdate)]
        public void OnBanchoMatchScoreUpdate(BanchoMatchScoreUpdateArgs args)
        {
            args.Pr.ActiveMatch?.Push(
                new MatchScoreUpdate(args.Pr.ActiveMatch.GetSlotIdByUserId(args.Pr.User.Id), args.Frame)
            );
        }
    }
}
