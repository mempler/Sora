using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class MatchPlayerFailed : IPacket
    {
        public PacketId Id => PacketId.ServerMatchPlayerFailed;

        public int UserId;

        public MatchPlayerFailed(int userId) => UserId = userId;

        public void ReadFromStream(MStreamReader sr) => throw new System.NotImplementedException();

        public void WriteToStream(MStreamWriter sw) => sw.Write(UserId);
    }
}
