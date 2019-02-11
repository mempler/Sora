using System.Diagnostics.Tracing;
using EventManager.Enums;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Events
{
    public class OnChannelLeaveEvent
    {
        [Handler(HandlerTypes.BanchoChannelLeave)]
        [Event(EventType.BanchoChannelLeave)]
        public void OnChannelLeave(Presence pr, string channelName)
        {
            Channel channel;
            switch (channelName)
            {
                case "#spectator":
                    channel = pr.Spectator?.SpecChannel;
                    break;
                case "#multiplayer":
                    channel = pr.JoinedRoom?.Channel;
                    break;
                default:
                    channel = LChannels.GetChannel(channelName);
                    break;
            }

            if (channel == null)
            {
                pr.Write(new ChannelRevoked(channelName));
                return;
            }

            channel.LeaveChannel(pr);

            channel.BoundStream?.Broadcast(new ChannelAvailable(channel));
        }
    }
}