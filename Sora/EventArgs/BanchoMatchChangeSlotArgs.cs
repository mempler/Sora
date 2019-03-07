using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchChangeSlotArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
        public int SlotId { get; set; }
    }
}