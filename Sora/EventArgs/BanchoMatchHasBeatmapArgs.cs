using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchHasBeatmapArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}