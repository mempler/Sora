using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class Announce : IPacket
    {
        public string Message;

        public Announce(string message) => Message = message;

        public PacketId Id => PacketId.ServerAnnounce;

        public void ReadFromStream(MStreamReader sr)
        {
            Message = sr.ReadString();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Message, false);
        }
    }
}
