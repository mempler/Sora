using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Events
{
    [EventClass]
    public class OnStopSpectatingEvent
    {
        [Event(EventType.BanchoStopSpectating)]
        public void OnStopSpectating(BanchoStopSpectatingArgs args)
        {
            if (args.pr?.Spectator == null) return;
            Presence opr = args.pr.Spectator.BoundPresence;

            opr.Write(new FellowSpectatorLeft(args.pr.User.Id));
            opr.Spectator.Broadcast(new SpectatorLeft(args.pr.User.Id));

            opr.Spectator.Left(args.pr);
            opr.Spectator.SpecChannel.LeaveChannel(args.pr);
            args.pr.Write(new ChannelRevoked(opr.Spectator.SpecChannel));

            args.pr.Spectator = null;

            if (opr.Spectator.JoinedUsers > 0) return;

            opr.Spectator.Left(opr);
            opr.Spectator.SpecChannel.LeaveChannel(opr);
            opr.Write(new ChannelRevoked(opr.Spectator.SpecChannel));
            opr.Spectator = null;
        }
    }
}