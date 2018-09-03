using Kaoiji.enums;
using Kaoiji.helpers;

namespace Kaoiji.packets
{
    public class Announce : Packet
    {
        public Announce() {
            PacketData = Binary.WriteOsuString(null);
            PacketID = PacketIDs.Server_Announce;
        }

        public Announce(string msg) {
            PacketData = Binary.WriteOsuString(msg);
            PacketID = PacketIDs.Server_Announce;
        }
    }
}
