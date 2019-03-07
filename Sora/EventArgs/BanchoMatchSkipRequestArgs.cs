using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchSkipRequestArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}