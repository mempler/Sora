using EventManager.Interfaces;
using Sora.Objects;
using Sora.Packets.Client;

namespace Sora.EventArgs
{
    public class BanchoSendUserStatusArgs : IEventArgs, INeedPresence
    {
        public UserStatus status;
        public Presence pr { get; set; }
    }
}