using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.Multiplayer.Match
{
    [EventClass]
    public class OnBanchoMatchJoinEvent
    {
        private readonly EventManager _ev;

        public OnBanchoMatchJoinEvent(EventManager ev)
        {
            _ev = ev;
        }

        [Event(EventType.BanchoMatchJoin)]
        public async void OnBanchoMatchJoin(BanchoMatchJoinArgs args)
        {
            Lobby.Self.TryGet(args.MatchId, out var room);
            if (room?.Join(args.Pr, args.Password.Replace(" ", "_")) == true)
                args.Pr.Push(new MatchJoinSuccess(room));
            else
                args.Pr.Push(new MatchJoinFail());

            room?.Update();

            await _ev.RunEvent(
                EventType.BanchoChannelJoin, new BanchoChannelJoinArgs {Pr = args.Pr, ChannelName = "#multiplayer"}
            );
        }
    }
}
