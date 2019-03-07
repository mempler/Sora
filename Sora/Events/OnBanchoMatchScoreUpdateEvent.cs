using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchScoreUpdateEvent
    {
        [Event(EventType.BanchoMatchScoreUpdate)]
        public void OnBanchoMatchScoreUpdate(BanchoMatchScoreUpdateArgs args)
        {
            args.pr.JoinedRoom?.Broadcast(new MatchScoreUpdate(args.pr.JoinedRoom.GetSlotIdByUserId(args.pr.User.Id), args.Frame));
        }
    }
}