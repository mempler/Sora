using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoStopSpectatingArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}