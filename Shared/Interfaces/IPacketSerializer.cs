using Shared.Enums;
using Shared.Helpers;

namespace Shared.Interfaces
{
    public interface IPacketSerializer
    {
        PacketId Id { get; }
        void ReadFromStream(MStreamReader sr);
        void WriteToStream(MStreamWriter sw);
    }
}
