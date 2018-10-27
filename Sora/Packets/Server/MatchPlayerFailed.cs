namespace Sora.Packets.Server
{
    using System;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class MatchPlayerFailed : IPacket
    {
        public int UserId;

        public MatchPlayerFailed(int userId) => UserId = userId;
        public PacketId Id => PacketId.ServerMatchPlayerFailed;

        public void ReadFromStream(MStreamReader sr) => throw new NotImplementedException();

        public void WriteToStream(MStreamWriter sw) => sw.Write(UserId);
    }
}
