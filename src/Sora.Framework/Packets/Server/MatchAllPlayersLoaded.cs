using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchAllPlayersLoaded : IPacket
    {
        public PacketId Id => PacketId.ServerMatchAllPlayersLoaded;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
        }
    }
}
