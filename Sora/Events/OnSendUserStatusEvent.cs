using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnSendUserStatusEvent
    {
        private readonly PacketStreamService _pss;

        public OnSendUserStatusEvent(PacketStreamService pss)
        {
            _pss = pss;
        }
        
        [Event(EventType.BanchoSendUserStatus)]
        public void OnSendUserStatus(BanchoSendUserStatusArgs args)
        {
            args.pr.Status = args.status;
            PacketStream main = _pss.GetStream("main");
            main.Broadcast(new UserPresence(args.pr));
            main.Broadcast(new HandleUpdate(args.pr));
        }
    }
}