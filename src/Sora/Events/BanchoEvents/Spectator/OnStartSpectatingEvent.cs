using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.Spectator
{
    [EventClass]
    public class OnStartSpectatingEvent
    {
        private readonly PresenceService _ps;

        public OnStartSpectatingEvent(PresenceService ps) => _ps = ps;

        [Event(EventType.BanchoStartSpectating)]
        public void OnStartSpectating(BanchoStartSpectatingArgs args)
        {
            if (!_ps.TryGet(args.SpectatorHostId, out var opr))
                return;
            
            if (opr.Spectator == null)
            {
                opr.Spectator = new Framework.Objects.Spectator(opr);
                opr.Spectator.Join(opr);
                opr.Push(new ChannelJoinSuccess(opr.Spectator.Channel));
            }

            args.Pr.Spectator = opr.Spectator;

            opr.Spectator.Join(args.Pr);
            args.Pr.Push(new ChannelJoinSuccess(opr.Spectator.Channel));
        }
    }
}
