namespace Sora.Packets.Server
{
    using System;
    using Shared.Enums;
    using Shared.Helpers;
    using Shared.Interfaces;

    public class MatchComplete : IPacket
    {
        public PacketId Id => PacketId.ServerMatchComplete;

        public void ReadFromStream(MStreamReader sr) => throw new NotImplementedException();
        public void WriteToStream(MStreamWriter sw) { }
    }
}
