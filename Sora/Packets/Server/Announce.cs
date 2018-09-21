using Sora.Enums;
using Sora.Helpers;

namespace Sora.Packets
{
    public class Announce : IPacketSerializer
    {
        public string Message;
        public Announce(string message) => Message = message;

        public PacketId Id => PacketId.ServerAnnounce;

        public void Read_from_stream(MStreamReader sr)
        {
            Message = sr.ReadString();
        }

        public void Write_to_stream(MStreamWriter sw)
        {
            sw.Write(Message);
        }
    }
}
