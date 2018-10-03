using System;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public struct MessageStruct
    {
        public string Username;
        public string Message;
        public string ChannelTarget;
        public int SenderId;
    }

    public class SendIrcMessage : IPacketSerializer
    {
        public PacketId Id => PacketId.ClientSendIrcMessage;

        public MessageStruct Msg;

        public SendIrcMessage(MessageStruct message)  => Msg = message;

        public void ReadFromStream(MStreamReader sr) => throw new NotImplementedException();

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Msg.Username);
            sw.Write(Msg.Message);
            sw.Write(Msg.ChannelTarget);
            sw.Write(Msg.SenderId);
        }
    }
}
