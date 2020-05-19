using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchLockArgs : IEventArgs, INeedPresence
    {
        public int SlotId { get; set; }
        public Presence Pr { get; set; }
    }
}
