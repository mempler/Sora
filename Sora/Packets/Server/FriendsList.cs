using System.Collections.Generic;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class FriendsList : IPacket
    {
        public PacketId Id => PacketId.ServerFriendsList;

        public List<int> FriendIds;

        public FriendsList(List<int> friendIds)
        {
            FriendIds = friendIds;
        }
        
        public void ReadFromStream(MStreamReader sr)
        {
            FriendIds = sr.ReadInt32List();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(FriendIds);
        }
    }
}