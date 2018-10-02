using Shared.Enums;
using Shared.Handlers;
using Sora.Objects;
using Sora.Packets.Server;

namespace Sora.Handler
{
    public class ChannelJoinHandler
    {
        [Handler(HandlerTypes.ClientChannelJoin)]
        public void OnChannelJoin(Presence pr, string channelName)
        {
            var channel = Channels.GetChannel(channelName);
            if (channel == null)
            {
                pr.Write(new ChannelRevoked(channelName));
                return;
            }

            if (channel.JoinChannel(pr))
                pr.Write(new ChannelJoinSuccess(channel));
        }
    }
}
