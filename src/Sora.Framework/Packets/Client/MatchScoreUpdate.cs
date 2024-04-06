using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class MatchScoreUpdate : IPacket
    {
        public ScoreFrame Frame;
        public PacketId Id => PacketId.ClientMatchScoreUpdate;

        public void ReadFromStream(MStreamReader sr)
        {
            Frame = sr.ReadData<ScoreFrame>();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
