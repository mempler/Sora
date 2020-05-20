using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchPartArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
