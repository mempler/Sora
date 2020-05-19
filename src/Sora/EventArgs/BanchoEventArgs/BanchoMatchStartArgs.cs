using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchStartArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
