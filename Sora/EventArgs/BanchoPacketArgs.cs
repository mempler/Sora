using EventManager.Interfaces;
using Shared.Enums;
using Shared.Helpers;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoPacketArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
        public PacketId PacketId;
        public MStreamReader Data;
    }
}