using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnPublicMessageEvent
    {
        private readonly ChannelService _cs;

        public OnPublicMessageEvent(ChannelService cs)
        {
            _cs = cs;
        }
        
        [Event(EventType.BanchoSendIrcMessage)]
        public void OnPublicMessage(BanchoSendIRCMessageArgs args)
        {
            Channel chan;
            switch (args.Message.ChannelTarget)
            {
                case "#spectator":
                    chan = args.pr.Spectator?.SpecChannel;
                    break;
                case "#multiplayer":
                    chan = null; // No multiplayer yet.
                    break;
                default:
                    chan = _cs.GetChannel(args.Message.ChannelTarget);
                    break;
            }

            if (chan == null)
            {
                args.pr.Write(new ChannelRevoked(args.Message.ChannelTarget));
                return;
            }

            chan.WriteMessage(args.pr, args.Message.Message);
        }
    }
}