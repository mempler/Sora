using Sora.Enums;

namespace Sora.Helpers
{
    public interface IPacketSerializer
    {
        PacketId Id { get; }
        void Read_from_stream(MStreamReader sr);
        void Write_to_stream(MStreamWriter sw);
    }
}
