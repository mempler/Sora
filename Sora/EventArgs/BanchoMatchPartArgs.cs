using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchPartArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}