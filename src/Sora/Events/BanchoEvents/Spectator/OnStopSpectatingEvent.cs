using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Packets.Server;

namespace Sora.Events.BanchoEvents.Spectator
{
    [EventClass]
    public class OnStopSpectatingEvent
    {
        [Event(EventType.BanchoStopSpectating)]
        public void OnStopSpectating(BanchoStopSpectatingArgs args)
        {
            if (args.Pr?.Spectator == null)
                return;
            
            var opr = args.Pr.Spectator.Host;

            opr.Push(new FellowSpectatorLeft(args.Pr.User.Id));
            opr.Spectator.Push(new SpectatorLeft(args.Pr.User.Id));

            opr.Spectator.Leave(args.Pr);
            opr.Spectator.Channel.Leave(args.Pr);
            args.Pr.Push(new ChannelRevoked(opr.Spectator.Channel));

            args.Pr.Spectator = null;

            if (opr.Spectator.SpectatorCount > 0)
                return;

            opr.Spectator.Leave(opr);
            opr.Spectator.Channel.Leave(opr);
            opr.Push(new ChannelRevoked(opr.Spectator.Channel));
            opr.Spectator = null;
        }
    }
}
