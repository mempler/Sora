namespace Sora.Packets.Client
{
    using System;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class MatchScoreUpdate : IPacket
    {
        public ScoreFrame Frame;
        public PacketId Id => PacketId.ClientMatchScoreUpdate;

        public void ReadFromStream(MStreamReader sr) => Frame = sr.ReadData<ScoreFrame>();

        public void WriteToStream(MStreamWriter sw) => throw new NotImplementedException();
    }
}
