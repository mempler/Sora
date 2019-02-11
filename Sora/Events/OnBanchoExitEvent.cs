using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoExitEvent
    {
        private readonly PresenceService _pcs;
        private readonly PacketStreamService _pss;

        public OnBanchoExitEvent(PresenceService pcs, PacketStreamService pss)
        {
            _pcs = pcs;
            _pss = pss;
        }
        
        [Event(EventType.BanchoExit)]
        public void OnBanchoExit(BanchoExitArgs args)
        {
            _pcs.EndPresence(args.pr, true);
            PacketStream mainStream = _pss.GetStream("main");

            mainStream?.Broadcast(new HandleUserQuit(new UserQuitStruct
            {
                UserId     = args.pr.User.Id,
                ErrorState = args.err
            }), args.pr);
        }
    }
}