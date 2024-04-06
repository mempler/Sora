using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchPlayerSkipped : IPacket
    {
        public int SlotId;

        public MatchPlayerSkipped(int slotId) => SlotId = slotId;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(SlotId);
        }

        public PacketId Id => PacketId.ServerMatchPlayerSkipped;
    }
}
