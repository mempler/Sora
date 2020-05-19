using Sora.Attributes;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework.Objects;
using Sora.Framework.Packets.Server;
using Sora.Services;

namespace Sora.Events.BanchoEvents.Chat
{
    [EventClass]
    public class OnChannelLeaveEvent
    {
        private readonly ChannelService _cs;

        public OnChannelLeaveEvent(ChannelService cs) => _cs = cs;

        [Event(EventType.BanchoChannelLeave)]
        public void OnChannelLeave(BanchoChannelLeaveArgs args)
        {
            Channel channel;
            switch (args.ChannelName)
            {
                case "#spectator":
                    channel = args.Pr.Spectator?.Channel;
                    break;
                case "#multiplayer":
                    channel = args.Pr.ActiveMatch?.Channel;
                    break;
                default:
                    _cs.TryGet(args.ChannelName, out channel);
                    break;
            }

            if (channel == null)
            {
                args.Pr.Push(new ChannelRevoked(args.ChannelName));
                return;
            }

            channel.Leave(args.Pr);

            channel.Push(new ChannelAvailable(channel));
            args.Pr.Push(new ChannelAvailable(channel));
        }
    }
}
