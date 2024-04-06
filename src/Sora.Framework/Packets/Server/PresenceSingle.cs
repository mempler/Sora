using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class PresenceSingle : IPacket
    {
        public int UserId;

        public PresenceSingle(int userId) => UserId = userId;

        public PacketId Id => PacketId.ServerUserPresenceSingle;

        public void ReadFromStream(MStreamReader sr)
        {
            UserId = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(UserId);
        }
    }
}
