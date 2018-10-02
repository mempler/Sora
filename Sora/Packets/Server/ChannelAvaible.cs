using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Objects;

namespace Sora.Packets.Server
{
    public class ChannelAvaible : IPacketSerializer
    {
        public PacketId Id => PacketId.ServerChannelAvailableAutojoin;

        public Channel Channel;

        public ChannelAvaible(Channel channel) => Channel = channel;

        public void ReadFromStream(MStreamReader sr) => throw new System.NotImplementedException();

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Channel.ChannelName, false);
            sw.Write(Channel.ChannelTopic, true);
            sw.Write((short)Channel.UserCount);
        }
    }
}
