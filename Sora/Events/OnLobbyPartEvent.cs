using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnLobbyPartEvent
    {
        private readonly PacketStreamService _ps;

        public OnLobbyPartEvent(PacketStreamService ps)
        {
            _ps = ps;
        }
        
        [Event(EventType.BanchoLobbyPart)]
        public void OnLobbyPart(BanchoLobbyPartArgs args)
        {
            _ps.GetStream("lobby").Left(args.pr);
        }
    }
}