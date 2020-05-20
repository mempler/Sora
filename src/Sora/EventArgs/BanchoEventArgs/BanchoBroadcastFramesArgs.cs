using Sora.Framework.Objects;
using Sora.Framework.Packets.Client;

namespace Sora.EventArgs.BanchoEventArgs
{
    public class BanchoBroadcastFramesArgs : IEventArgs, INeedPresence
    {
        public SpectatorFrame Frames;
        public Presence Pr { get; set; }
    }
}
