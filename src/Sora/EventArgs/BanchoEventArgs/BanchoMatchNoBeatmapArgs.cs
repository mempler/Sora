using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchNoBeatmapArgs : IEventArgs, INeedPresence
    {
        public Presence Pr { get; set; }
    }
}
