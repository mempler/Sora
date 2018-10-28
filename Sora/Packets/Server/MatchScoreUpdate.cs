namespace Sora.Packets.Server
{
    using Client;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class MatchScoreUpdate : IPacket
    {
        public ScoreFrame Frame;

        public MatchScoreUpdate(int slotId, ScoreFrame frame)
        {
            Frame = frame;
            Frame.Id = (byte) slotId;
        }

        public PacketId Id => PacketId.ServerMatchScoreUpdate;

        public void ReadFromStream(MStreamReader sr) => Frame = sr.ReadData<ScoreFrame>();

        public void WriteToStream(MStreamWriter sw) => sw.Write(Frame);
    }
}
