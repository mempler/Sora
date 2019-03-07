using System.Collections.Generic;
using Sora.Objects;

namespace Sora.Services
{
    public class ChannelService
    {
        public readonly Dictionary<string, Channel> Channels = new Dictionary<string, Channel>();
        public readonly List<Channel> ChannelsAutoJoin = new List<Channel>();

        public ChannelService(PacketStreamService streamService)
        {
            AddChannel(new Channel("#osu", "Osu! default channel.", streamService.GetStream("main"), autoJoin: true));
            AddChannel(new Channel("#announce", "Osu! default channel.", streamService.GetStream("main"),
                                   readOnly: true,
                                   autoJoin: true));
            AddChannel(new Channel("#userlog", "Osu! default channel.", streamService.GetStream("main"),
                                   readOnly: true));
            AddChannel(new Channel("#lobby", "Osu! default channel", streamService.GetStream("main")));
            AddChannel(new Channel("#admin", "Admin. is an administration channel.", streamService.GetStream("admin"),
                                   adminOnly: true, autoJoin: true));
        }
        
        public void AddChannel(Channel channel)
        {
            if (channel.AutoJoin)
                ChannelsAutoJoin.Add(channel);
            
            Channels.TryAdd(channel.ChannelName, channel);
        }

        public void RemoveChannel(Channel channel)
        {
            ChannelsAutoJoin.Remove(channel);
            Channels.Remove(channel.ChannelName);
        }

        public void RemoveChannel(string channelName)
        {
            Channel chan = GetChannel(channelName);
            if (chan == null) return;
            ChannelsAutoJoin.Remove(chan);
            Channels.Remove(chan.ChannelName);
        }

        public Channel GetChannel(string channelName)
        {
            Channels.TryGetValue(channelName, out Channel x);
            return x;
        }
    }
}