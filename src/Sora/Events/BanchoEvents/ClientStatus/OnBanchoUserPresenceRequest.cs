using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.ClientStatus
{
    [EventClass]
    public class OnBanchoUserPresenceRequest
    {
        private readonly PresenceService _ps;

        public OnBanchoUserPresenceRequest(PresenceService ps)
        {
            _ps = ps;
        }
        
        [Event(EventType.BanchoUserPresenceRequest)]
        public void RequestBanchoPresence(BanchoClientUserPresenceRequestArgs args)
        {
            foreach (var id in args.UserIds)
            {
                if (_ps.TryGet(id, out var pr))
                    args.Pr.Push(new UserPresence(pr));
            }
        }
    }
}