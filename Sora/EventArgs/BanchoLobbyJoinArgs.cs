using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoLobbyJoinArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
    }
}