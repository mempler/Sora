using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public struct MessageStruct
    {
        public string Username;
        public string Message;
        public string ChannelTarget;
        public int SenderId;
    }

    public class SendIrcMessage : IPacket
    {
        public MessageStruct Msg;

        public SendIrcMessage(MessageStruct message) => Msg = message;

        public PacketId Id => PacketId.ServerSendMessage;

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
