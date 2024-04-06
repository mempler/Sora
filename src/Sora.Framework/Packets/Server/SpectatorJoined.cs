using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class SpectatorJoined : IPacket
    {
        public readonly int UserId;

        public SpectatorJoined(int userid) => UserId = userid;

        public PacketId Id => PacketId.ServerSpectatorJoined;

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
