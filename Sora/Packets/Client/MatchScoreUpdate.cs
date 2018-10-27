using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Client
{
    public class MatchScoreUpdate : IPacket
    {
        public PacketId Id => PacketId.ClientMatchScoreUpdate;

        public ScoreFrame Frame;

        public void ReadFromStream(MStreamReader sr) => Frame = sr.ReadData<ScoreFrame>();

        public void WriteToStream(MStreamWriter sw) => throw new System.NotImplementedException();
    }
}
