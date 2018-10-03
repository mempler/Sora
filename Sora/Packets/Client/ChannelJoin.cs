using System;
using System.Collections.Generic;
using System.Text;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Client
{
    public class ChannelJoin : IPacketSerializer
    {
        public PacketId Id => PacketId.ClientChannelJoin;

        public string ChannelName;

        public void ReadFromStream(MStreamReader sr) => ChannelName = sr.ReadString();

        public void WriteToStream(MStreamWriter sw) => throw new NotImplementedException();
    }
}
