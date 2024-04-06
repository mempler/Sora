using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class FellowSpectatorLeft : IPacket
    {
        public int UserId;

        public FellowSpectatorLeft(int userid) => UserId = userid;

        public PacketId Id => PacketId.ServerFellowSpectatorLeft;

        public void ReadFromStream(MStreamReader sr)
        {
            throw new NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(UserId);
        }
    }
}
