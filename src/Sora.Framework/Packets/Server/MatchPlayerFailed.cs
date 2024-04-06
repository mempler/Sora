using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class MatchPlayerFailed : IPacket
    {
        public int SlotId;

        public MatchPlayerFailed(int slotId) => SlotId = slotId;

        public PacketId Id => PacketId.ServerMatchPlayerFailed;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(SlotId);
        }
    }
}
