using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoCantSpectateArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}