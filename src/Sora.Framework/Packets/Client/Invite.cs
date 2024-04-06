using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class Invite : IPacket
    {
        public int UserId;
        public PacketId Id => PacketId.ClientInvite;

        public void ReadFromStream(MStreamReader sr)
        {
            UserId = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
