using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchCreateArgs : IEventArgs, INeedPresence
    {
        public MultiplayerRoom room;
        public Presence pr { get; set; }
    }
}