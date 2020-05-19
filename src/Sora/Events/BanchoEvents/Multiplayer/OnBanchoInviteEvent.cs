using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Services;

namespace Sora.Events.BanchoEvents.Multiplayer
{
    [EventClass]
    public class OnBanchoInviteEvent
    {
        private readonly PresenceService _ps;

        public OnBanchoInviteEvent(PresenceService ps) => _ps = ps;

        [Event(EventType.BanchoInvite)]
        public void OnBanchoInvite(BanchoInviteArgs args)
        {
            if (args.Pr.ActiveMatch == null)
                return;

            if (!_ps.TryGet(args.UserId, out var opr))
                return;

            args.Pr.ActiveMatch.Invite(args.Pr, opr);
        }
    }
}
