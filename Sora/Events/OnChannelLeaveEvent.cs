using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnChannelLeaveEvent
    {
        private readonly ChannelService _cs;

        public OnChannelLeaveEvent(ChannelService cs)
        {
            _cs = cs;
        }
        
        [Event(EventType.BanchoChannelLeave)]
        public void OnChannelLeave(BanchoChannelLeaveArgs args)
        {
            Channel channel;
            switch (args.ChannelName)
            {
                case "#spectator":
                    channel = args.pr.Spectator?.SpecChannel;
                    break;
                case "#multiplayer":
                    channel = args.pr.JoinedRoom?.Channel;
                    break;
                default:
                    channel = _cs.GetChannel(args.ChannelName);
                    break;
            }

            if (channel == null)
            {
                args.pr.Write(new ChannelRevoked(args.ChannelName));
                return;
            }

            channel.LeaveChannel(args.pr);

            channel.BoundStream?.Broadcast(new ChannelAvailable(channel));
        }
    }
}