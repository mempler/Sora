using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchTransferHostArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
        public int SlotId { get; set; }
    }
}