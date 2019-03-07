using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoPongArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}