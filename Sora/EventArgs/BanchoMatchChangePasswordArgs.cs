using EventManager.Interfaces;
using Sora.Objects;

namespace Sora.EventArgs
{
    public class BanchoMatchChangePasswordArgs : INeedPresence, IEventArgs
    {
        public Presence pr { get; set; }
        public MultiplayerRoom room { get; set; }
    }
}