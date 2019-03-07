using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchFailedArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
    }
}