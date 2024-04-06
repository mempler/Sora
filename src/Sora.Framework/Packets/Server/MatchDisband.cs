using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchDisband : IPacket
    {
        public Match Match;

        public MatchDisband(Match match) => Match = match;

        public PacketId Id => PacketId.ServerMatchDisband;

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
