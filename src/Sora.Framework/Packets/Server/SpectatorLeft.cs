using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Server
{
    public class SpectatorLeft : IPacket
    {
        public readonly int UserId;

        public SpectatorLeft(int userid) => UserId = userid;

        public PacketId Id => PacketId.ServerSpectatorLeft;

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
