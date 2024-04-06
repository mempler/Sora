using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class MatchChangePassword : IPacket
    {
        public Match Match;
        public PacketId Id => PacketId.ClientMatchChangePassword;

        public void ReadFromStream(MStreamReader sr)
        {
            (Match = new Match()).ReadFromStream(sr);
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
