using EventManager.Interfaces;
using Sora.Objects;
using Sora.Packets.Client;

namespace Sora.EventArgs
{
    public class BanchoBroadcastFramesArgs : IEventArgs, INeedPresence
    {
        public SpectatorFrame frames;
        public Presence pr { get; set; }
    }
}