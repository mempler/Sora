using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchLoadCompleteArgs : INeedPresence, IEventArgs
    {
        public Presence Pr { get; set; }
    }
}
