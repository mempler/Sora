using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchHasBeatmapArgs : INeedPresence, IEventArgs
    {
        public Presence Pr { get; set; }
    }
}
