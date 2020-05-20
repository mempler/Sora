using Sora.Framework.Objects;
using Sora.Framework.Objects.Multiplayer;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchChangePasswordArgs : INeedPresence, IEventArgs
    {
        public Match Room { get; set; }
        public Presence Pr { get; set; }
    }
}
