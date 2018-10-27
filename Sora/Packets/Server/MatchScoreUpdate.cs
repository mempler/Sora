using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;
using Sora.Packets.Client;

namespace Sora.Packets.Server
{
    public class MatchScoreUpdate : IPacket
    {
        public PacketId Id => PacketId.ServerMatchScoreUpdate;

        public int SlotId;
        public ScoreFrame Frame;
        
        public MatchScoreUpdate(int slotId, ScoreFrame frame)
        {
            Frame = frame;
            SlotId = slotId;
        }
        
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