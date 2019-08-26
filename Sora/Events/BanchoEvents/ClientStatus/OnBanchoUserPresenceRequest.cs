using System.Linq;
using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;
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
            foreach (var opr in args.userIds.Select(id => _ps.GetPresence(id)).Where(opr => opr != null))
            {
                args.pr.Write(new UserPresence(opr));
            }
        }
    }
}