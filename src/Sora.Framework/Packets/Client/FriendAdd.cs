using System;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class FriendAdd : IPacket
    {
        public int FriendId;
        public PacketId Id => PacketId.ClientFriendAdd;

        public void ReadFromStream(MStreamReader sr)
        {
            FriendId = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new NotImplementedException();
        }
    }
}
