using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoStartSpectatingArgs : IEventArgs, INeedPresence
    {
        public int SpectatorHostID;
        public Presence pr { get; set; }
    }
}