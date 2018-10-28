namespace Sora.Packets.Server
{
    using System;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class MatchPlayerSkipped : IPacket
    {
        public int SlotId;

        public MatchPlayerSkipped(int slotId) => SlotId = slotId;
        public void ReadFromStream(MStreamReader sr) => throw new NotImplementedException();
        public void WriteToStream(MStreamWriter sw) => sw.Write(SlotId);
        public PacketId Id => PacketId.ServerMatchPlayerSkipped;
    }
}
