using Sora.Framework.Utilities;

namespace Sora.Framework
{
    public interface IPacketPusher
    {
        void Push(IPacket packet);
    }
}