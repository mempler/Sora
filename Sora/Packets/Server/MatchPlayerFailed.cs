using System;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class MatchPlayerFailed : IPacket
    {
        public int SlotId;

        public MatchPlayerFailed(int slotId)
        {
            SlotId = slotId;
        }

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