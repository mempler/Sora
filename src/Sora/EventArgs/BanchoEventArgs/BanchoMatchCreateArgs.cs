using Sora.Framework.Objects;
using Sora.Framework.Objects.Multiplayer;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchCreateArgs : IEventArgs, INeedPresence
    {
        public Match Room;
        public Presence Pr { get; set; }
    }
}
