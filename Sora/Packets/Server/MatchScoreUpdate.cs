namespace Sora.Packets.Server
{
    using Client;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class MatchScoreUpdate : IPacket
    {
        public ScoreFrame Frame;

        public int SlotId;

        public MatchScoreUpdate(int slotId, ScoreFrame frame)
        {
            Frame = frame;
            SlotId = slotId;
        }

        public PacketId Id => PacketId.ServerMatchScoreUpdate;

        public void ReadFromStream(MStreamReader sr) => Frame = sr.ReadData<ScoreFrame>();

        public void WriteToStream(MStreamWriter sw) => sw.Write(Frame);
    }
}
