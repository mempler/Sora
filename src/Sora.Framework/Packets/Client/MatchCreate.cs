using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class MatchCreate : IPacket
    {
        public Match Match;
        public PacketId Id => PacketId.ClientMatchCreate;

        public void ReadFromStream(MStreamReader sr)
        {
            Match = new Match();
            Match.ReadFromStream(sr);
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
