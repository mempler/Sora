using EventManager.Interfaces;
using Shared.Enums;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchChangeModsArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
        public Mod mods { get; set; }
    }
}