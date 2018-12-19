using System;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class MatchSkip : IPacket
    {
        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
        }

        public PacketId Id => PacketId.ServerMatchSkip;
    }
}