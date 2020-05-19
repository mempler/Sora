using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.Multiplayer
{
    [EventClass]
    public class OnLobbyJoinEvent
    {

        [Event(EventType.BanchoLobbyJoin)]
        public void OnLobbyJoin(BanchoLobbyJoinArgs args)
        {
            foreach (var room in Lobby.Self.Rooms)
                args.Pr.Push(new MatchNew(room));

            Lobby.Self.Leave(args.Pr);
            Lobby.Self.Join(args.Pr);
        }
    }
}
