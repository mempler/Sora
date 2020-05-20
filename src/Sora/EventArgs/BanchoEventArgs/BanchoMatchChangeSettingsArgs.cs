using Sora.Framework.Objects;
using Sora.Framework.Objects.Multiplayer;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchChangeSettingsArgs : IEventArgs, INeedPresence
    {
        public Match Room { get; set; }
        public Presence Pr { get; set; }
    }
}
