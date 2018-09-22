using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class ProtocolNegotiation : IPacketSerializer
    {
        public PacketId Id => PacketId.ServerProtocolNegotiation;

        public uint Protocol = 19;

        public void ReadFromStream(MStreamReader sr) => Protocol = sr.ReadUInt32();

        public void WriteToStream(MStreamWriter sw) => sw.Write(Protocol);
    }
}
