using Shared.Enums;
using Shared.Helpers;
using Shared.Interfaces;

namespace Sora.Packets.Client
{
    public class StartSpectating : IPacket
    {
        public PacketId Id => PacketId.ClientStartSpectating;

        public int ToSpectateId;
        
        public void ReadFromStream(MStreamReader sr)
        {
            ToSpectateId = sr.ReadInt32();
        }

        public void WriteToStream(MStreamWriter sw)
        {
            throw new System.NotImplementedException();
        }
    }
}