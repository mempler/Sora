using Sora.Framework.Enums;
using Sora.Framework.Packets.Server;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class SendIrcMessage : IPacket
    {
        public MessageStruct Msg;
        public PacketId Id => PacketId.ClientSendIrcMessage;

        public void ReadFromStream(MStreamReader sr)
        {
            Msg = new MessageStruct
            {
                Username = sr.ReadString(),
                Message = sr.ReadString(),
                ChannelTarget = sr.ReadString(),
                SenderId = sr.ReadInt32()
            };
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Msg.Username);
            sw.Write(Msg.Message);
            sw.Write(Msg.ChannelTarget);
            sw.Write(Msg.SenderId);
        }
    }
}
