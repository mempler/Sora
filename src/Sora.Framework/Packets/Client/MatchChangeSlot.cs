using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class MatchChangeSlot : IPacket
    {
        public int SlotId;
        public PacketId Id => PacketId.ClientMatchChangeSlot;

        public void ReadFromStream(MStreamReader sr)
        {
            SlotId = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
