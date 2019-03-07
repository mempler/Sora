using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchLoadCompleteArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}