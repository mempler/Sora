using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;
using Sora.Packets.Server;

namespace Sora.Events
{
    [EventClass]
    public class OnBroadcastFramesEvent
    {
        [Event(EventType.BanchoBroadcastFrames)]
        public void OnBroadcastFrames(BanchoBroadcastFramesArgs args)
        {
            args.pr.Spectator?.Broadcast(new SpectatorFrames(args.frames), args.pr);
        }
    }
}