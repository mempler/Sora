using System;
using Sora.Framework.Enums;
using Sora.Framework.Objects.Multiplayer;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchJoinSuccess : IPacket
    {
        public Match Match;

        public MatchJoinSuccess(Match match) => Match = match;

        public PacketId Id => PacketId.ServerMatchJoinSuccess;

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
