using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchSkipRequestArgs : INeedPresence, IEventArgs
    {
        public Presence Pr { get; set; }
    }
}
