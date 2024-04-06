using System.Collections.Generic;
using Sora.Framework.Enums;
using Sora.Framework.Utilities;

namespace Sora.Framework.Packets.Client
{
    public class UserStatsRequest : IPacket
    {
        public List<int> Userids;
        public PacketId Id => PacketId.ClientUserStatsRequest;

        public void ReadFromStream(MStreamReader sr)
        {
            Userids = sr.ReadInt32List();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(Userids);
        }
    }
}
