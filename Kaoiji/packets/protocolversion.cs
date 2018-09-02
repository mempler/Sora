using System;

using Kaoiji.enums;

namespace Kaoiji.packets
{
    public class ProtocolVersion : Packet
    {
        public ProtocolVersion()
        {
            PacketID = PacketIDs.Server_ProtocolNegotiation;
            PacketData = BitConverter.GetBytes(Program.ProtocolVersion);
        }
    }
}
