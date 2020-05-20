using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects.Multiplayer;

namespace Sora.Events.BanchoEvents.Multiplayer
{
    [EventClass]
    public class OnLobbyPartEvent
    {
        [Event(EventType.BanchoLobbyPart)]
        public void OnLobbyPart(BanchoLobbyPartArgs args)
        {
            Lobby.Self.Leave(args.Pr);
        }
    }
}
