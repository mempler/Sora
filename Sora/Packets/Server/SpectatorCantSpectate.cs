using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Server
{
    public class SpectatorCantSpectate : IPacket
    {
        public PacketId Id => PacketId.ServerSpectatorCantSpectate;

        public int UserId;
        
        public SpectatorCantSpectate(int userId)
        {
            UserId = userId;
        }
        
        public void ReadFromStream(MStreamReader sr)
        {
            throw new System.NotImplementedException();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            sw.Write(UserId);
        }
    }
}