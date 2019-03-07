using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchReadyArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}