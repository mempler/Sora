using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchNew : IPacket
    {
        public Match Match;

        public MatchNew(Match match) => Match = match;

        public PacketId Id => PacketId.ServerMatchNew;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Match);
        }
    }
}
