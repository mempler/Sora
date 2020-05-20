using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchNotReadyArgs : INeedPresence, IEventArgs
    {
        public Presence Pr { get; set; }
    }
}
