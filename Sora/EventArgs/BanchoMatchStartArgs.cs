using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchStartArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}