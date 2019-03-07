using System.Collections.Generic;
using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnLobbyJoinEvent
    {
        private readonly PacketStreamService _ps;
        private readonly MultiplayerService _ms;

        public OnLobbyJoinEvent(PacketStreamService ps, MultiplayerService ms)
        {
            _ps = ps;
            _ms = ms;
        }
        
        [Event(EventType.BanchoLobbyJoin)]
        public void OnLobbyJoin(BanchoLobbyJoinArgs args)
        {
            PacketStream                 lobbyStream = _ps.GetStream("lobby");
            IEnumerable<MultiplayerRoom> rooms       = _ms.GetRooms();
            foreach (MultiplayerRoom room in rooms)
                args.pr.Write(new MatchNew(room));

            lobbyStream.Join(args.pr);
        }
    }
}