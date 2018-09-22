using System.Collections.Generic;
using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class PresenceBundle : IPacketSerializer
    {
        public PacketId Id => PacketId.ServerUserPresenceBundle;

        public List<int> UserIds;

        public PresenceBundle(List<int> userIds)
        {
            UserIds = userIds;
        }

        public void ReadFromStream(MStreamReader sr) => sr.ReadInt32List();

        public void WriteToStream(MStreamWriter sw) => sw.Write(UserIds);
    }
}
