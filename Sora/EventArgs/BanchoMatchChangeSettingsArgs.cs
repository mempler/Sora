using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchChangeSettingsArgs : IEventArgs, INeedPresence
    {
        public Presence pr { get; set; }
        public MultiplayerRoom room { get; set; }
    }
}
