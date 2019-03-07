using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchJoinEvent
    {
        private readonly MultiplayerService _ms;

        public OnBanchoMatchJoinEvent(MultiplayerService ms)
        {
            _ms = ms;
        }
    
        [Event(EventType.BanchoMatchJoin)]
        public void OnBanchoMatchJoin(BanchoMatchJoinArgs args)
        {
            MultiplayerRoom room = _ms.GetRoom(args.matchId);
            if (room != null && room.Join(args.pr, args.password.Replace(" ", "_")))
                args.pr.Write(new MatchJoinSuccess(room));
            else
                args.pr.Write(new MatchJoinFail());

            room?.Update();
        }
    }
}