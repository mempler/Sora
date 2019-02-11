using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;

namespace Sora.Events
{
    [EventClass]
    public class OnChannelJoinEvent
    {
        private readonly ChannelService _cs;

        public OnChannelJoinEvent(ChannelService cs)
        {
            _cs = cs;
        }
        
        [Event(EventType.BanchoChannelJoin)]
        public void OnChannelJoin(BanchoChannelJoinArgs args)
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

            channel.LeaveChannel(args.pr); // leave channel before joining to fix some Issues.

            if (channel.JoinChannel(args.pr))
                args.pr.Write(new ChannelJoinSuccess(channel));

            channel.BoundStream?.Broadcast(new ChannelAvailable(channel));
        }
    }
}