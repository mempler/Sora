using Sora.Enums;

namespace Sora.Helpers
{
    public interface IPacketSerializer
    {
        PacketId Id { get; }
        void ReadFromStream(MStreamReader sr);
        void WriteToStream(MStreamWriter sw);
    }
}
