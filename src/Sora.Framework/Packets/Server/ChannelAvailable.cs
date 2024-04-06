using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class ChannelAvailable : IPacket
    {
        public Channel Channel;

        public ChannelAvailable(Channel channel) => Channel = channel;

        public PacketId Id => PacketId.ServerChannelAvailable;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Channel.Name, false);
            sw.Write(Channel.Topic, true);
            sw.Write((short) Channel.UserCount);
        }
    }
}
