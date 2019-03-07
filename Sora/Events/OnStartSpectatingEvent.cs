using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnStartSpectatingEvent
    {
        private readonly PresenceService _ps;

        public OnStartSpectatingEvent(PresenceService ps)
        {
            _ps = ps;
        }
        
        [Event(EventType.BanchoStartSpectating)]
        public void OnStartSpectating(BanchoStartSpectatingArgs args)
        {
            Presence opr = _ps.GetPresence(args.SpectatorHostID);
            if (opr == null) return;
            if (opr.Spectator == null)
            {
                opr.Spectator = new SpectatorStream($"spec-{args.SpectatorHostID}", opr);
                opr.Spectator.Join(opr);
                opr.Write(new ChannelJoinSuccess(opr.Spectator.SpecChannel));
            }

            args.pr.Spectator = opr.Spectator;

            opr.Spectator.Join(args.pr);
            if (opr.Spectator.SpecChannel.JoinChannel(args.pr))
                args.pr.Write(new ChannelJoinSuccess(opr.Spectator.SpecChannel));

            opr.Spectator.Broadcast(new FellowSpectatorJoined(args.pr.User.Id));
            opr.Write(new SpectatorJoined(args.pr.User.Id));
        }
    }
}