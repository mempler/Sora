using Sora.Framework.Enums;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoPacketArgs : IEventArgs, INeedPresence
    {
        public MStreamReader Data;
        public PacketId PacketId;
        public Presence Pr { get; set; }
    }
}
