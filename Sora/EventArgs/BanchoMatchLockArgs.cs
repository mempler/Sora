using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchLockArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
        public int SlotId { get; set; }
    }
}