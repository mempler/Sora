using Sora.Framework.Enums;
using Sora.Framework.Packets.Client;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchScoreUpdate : IPacket
    {
        public ScoreFrame Frame;

        public MatchScoreUpdate(int slotId, ScoreFrame frame)
        {
            Frame = frame;
            Frame.Id = (byte) slotId;
        }

        public PacketId Id => PacketId.ServerMatchScoreUpdate;

        public void ReadFromStream(MStreamReader sr)
        {
            Frame = sr.ReadData<ScoreFrame>();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Frame);
        }
    }
}
