using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class FellowSpectatorJoined : IPacket
    {
        public int UserId;

        public FellowSpectatorJoined(int userid) => UserId = userid;

        public PacketId Id => PacketId.ServerFellowSpectatorJoined;

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
