using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoInviteEvent
    {
        private readonly PresenceService _ps;

        public OnBanchoInviteEvent(PresenceService ps)
        {
            _ps = ps;
        }
        
        [Event(EventType.BanchoInvite)]
        public void OnBanchoInvite(BanchoInviteArgs args)
        {
            if (args.pr.JoinedRoom == null) return;
            Presence opr = _ps.GetPresence(args.UserId);
            if (opr == null) return;
            args.pr.JoinedRoom.Invite(args.pr, opr);
        }
    }
}