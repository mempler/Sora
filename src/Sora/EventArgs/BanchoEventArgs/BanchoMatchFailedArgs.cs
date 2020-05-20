using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchFailedArgs : INeedPresence, IEventArgs
    {
        public Presence Pr { get; set; }
    }
}
