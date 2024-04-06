using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class ChannelJoinSuccess : IPacket
    {
        private Channel Channel;

        public ChannelJoinSuccess(Channel channel) => Channel = channel;

        public PacketId Id => PacketId.ServerChannelJoinSuccess;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Channel.Name);
        }
    }
}
