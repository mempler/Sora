using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchJoinArgs : IEventArgs, INeedPresence
    {
        public int matchId;
        public string password;
        public Presence pr { get; set; }
    }
}