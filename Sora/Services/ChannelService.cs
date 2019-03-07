#region LICENSE
/*
    Sora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

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