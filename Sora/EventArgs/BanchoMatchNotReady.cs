using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchNotReadyArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}