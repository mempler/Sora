using EventManager.Attributes;
using EventManager.Enums;
using Sora.EventArgs;

namespace Sora.Events
{
    [EventClass]
    public class OnBanchoMatchStartEvent
    {
        [Event(EventType.BanchoMatchStart)]
        public void OnBanchoMatchStart(BanchoMatchStartArgs args)
        {
            if (args.pr.JoinedRoom == null || args.pr.JoinedRoom.HostId != args.pr.User.Id) return;
            args.pr.JoinedRoom.Start();
        }
    }
}