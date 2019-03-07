using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchNoBeatmapArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}