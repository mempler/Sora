using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoLobbyPartArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}