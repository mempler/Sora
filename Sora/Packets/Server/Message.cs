using System;
using System.Collections.Generic;
using System.Text;
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
    public class Message : IPacketSerializer
    {
        public PacketId Id => PacketId.ClientSendIrcMessage;

        public MessageStruct Msg;

        public Message(MessageStruct message)  => Msg = message;

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
