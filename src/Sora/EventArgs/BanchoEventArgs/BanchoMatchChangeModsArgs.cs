using Sora.Framework.Enums;
using Sora.Framework.Objects;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoMatchChangeModsArgs : IEventArgs, INeedPresence
    {
        public Mod Mods { get; set; }
        public Presence Pr { get; set; }
    }
}
