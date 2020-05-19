using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchChangeSlotArgs : INeedPresence, IEventArgs
    {
        public int SlotId { get; set; }
        public Presence Pr { get; set; }
    }
}
